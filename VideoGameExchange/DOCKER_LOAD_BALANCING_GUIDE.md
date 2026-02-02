# Video Game Exchange - Docker Load Balancing Setup Guide

## Overview
This setup containerizes your Video Game Exchange API with:
- **2 API instances** (api1 and api2)
- **1 MongoDB instance** (shared database)
- **1 NGINX load balancer** (distributes traffic between API instances)

## Architecture

```
Client Request
      ↓
NGINX Load Balancer (localhost:8080)
      ↓
   Round-Robin Distribution
      ↓
   ┌──────────────┐
   ↓              ↓
API Instance 1  API Instance 2
   ↓              ↓
   └──────────────┘
         ↓
    MongoDB Database
```

## Files Created

1. **docker-compose.yml** - Orchestrates all services
2. **nginx.conf** - NGINX load balancer configuration
3. **appsettings.Production.json** - Production environment settings
4. **Program.cs** (modified) - Added instance identification logging

## Step-by-Step Setup

### Step 1: Build and Start the Services

```powershell
# Navigate to your project directory
cd c:\Classes\DistributedSystems\VideoGameExchange

# Build and start all services
docker-compose up --build
```

This command will:
- Build the API Docker image
- Pull MongoDB and NGINX images
- Start all 5 containers (mongodb, api1, api2, nginx)
- Create a shared network for communication

### Step 2: Verify Services are Running

```powershell
# Check running containers
docker-compose ps

# You should see:
# - videogame-mongodb (port 27017)
# - videogame-api1 (internal only)
# - videogame-api2 (internal only)
# - videogame-nginx (port 8080)
```

### Step 3: Test Load Balancing

#### Test with curl (multiple requests):
```powershell
# Make multiple requests to see load balancing
for ($i=1; $i -le 10; $i++) { 
    curl -I http://localhost:8080/api/games
    Write-Host "Request $i completed"
    Start-Sleep -Milliseconds 500
}
```

#### Or test with PowerShell:
```powershell
# Make 10 requests and check which instance handled each
for ($i=1; $i -le 10; $i++) {
    $response = Invoke-WebRequest -Uri http://localhost:8080/api/games -Method Get
    Write-Host "Request $i - Instance: $($response.Headers.'X-API-Instance')"
}
```

### Step 4: View Logs to Verify Distribution

#### View all logs:
```powershell
docker-compose logs -f
```

#### View API instance 1 logs only:
```powershell
docker-compose logs -f api1
```

#### View API instance 2 logs only:
```powershell
docker-compose logs -f api2
```

#### View NGINX logs:
```powershell
docker-compose logs -f nginx
```

## What to Look For in Logs

### API Instance Logs
You'll see entries like:
```
api1    | [API-Instance-1] [a3f5e8c2] Handling GET request to /api/games from 172.18.0.5
api1    | [API-Instance-1] [a3f5e8c2] Completed GET request to /api/games with status 200
```

```
api2    | [API-Instance-2] [b7d4f1a9] Handling GET request to /api/games from 172.18.0.5
api2    | [API-Instance-2] [b7d4f1a9] Completed GET request to /api/games with status 200
```

### NGINX Logs
You'll see entries showing which backend server handled each request:
```
nginx   | [30/Jan/2026:14:23:45 +0000] 172.18.0.1 - - Server: 172.18.0.3:8080 | Request: "GET /api/games HTTP/1.1" | Status: 200
nginx   | [30/Jan/2026:14:23:46 +0000] 172.18.0.1 - - Server: 172.18.0.4:8080 | Request: "GET /api/games HTTP/1.1" | Status: 200
```

## Response Headers

Each response includes headers showing load balancing info:
- **X-API-Instance**: Shows which API instance handled the request (API-Instance-1 or API-Instance-2)
- **X-Upstream-Server**: Shows the internal Docker address of the backend server
- **X-Load-Balancer**: Confirms NGINX is handling the load balancing

## Load Balancing Strategy

NGINX uses **round-robin** by default, which means:
- Request 1 → API Instance 1
- Request 2 → API Instance 2
- Request 3 → API Instance 1
- Request 4 → API Instance 2
- And so on...

## Useful Commands

### Start services in background:
```powershell
docker-compose up -d
```

### Stop services:
```powershell
docker-compose down
```

### Stop and remove volumes (clean slate):
```powershell
docker-compose down -v
```

### Rebuild after code changes:
```powershell
docker-compose up --build
```

### Scale to more instances (e.g., 3 API instances):
```powershell
docker-compose up --scale api1=2 --scale api2=1 -d
```

### View resource usage:
```powershell
docker stats
```

### Execute commands in a container:
```powershell
# Access MongoDB shell
docker exec -it videogame-mongodb mongosh

# Access API container
docker exec -it videogame-api1 bash
```

## Testing Endpoints

### Health Check:
```powershell
Invoke-WebRequest http://localhost:8080/health
```

### Get Games:
```powershell
Invoke-WebRequest http://localhost:8080/api/games
```

### Register User:
```powershell
$body = @{
    username = "testuser"
    email = "test@example.com"
    password = "Test123!"
} | ConvertTo-Json

Invoke-WebRequest -Uri http://localhost:8080/api/auth/register `
                  -Method Post `
                  -Body $body `
                  -ContentType "application/json"
```

## Troubleshooting

### If containers fail to start:
```powershell
# Check logs for errors
docker-compose logs

# Check specific service
docker-compose logs mongodb
```

### If MongoDB connection fails:
- Wait 10-15 seconds after starting for MongoDB health check to pass
- Check MongoDB is healthy: `docker-compose ps`

### If NGINX shows 502 Bad Gateway:
- Verify API instances are running: `docker-compose ps`
- Check API logs: `docker-compose logs api1 api2`

### Port already in use:
```powershell
# Check what's using port 8080
netstat -ano | findstr :8080

# Kill the process or change the port in docker-compose.yml
```

## Performance Testing

To really see the load distribution, you can use a load testing tool:

```powershell
# Install Apache Bench (if needed)
# Or use PowerShell to generate load:

$jobs = 1..100 | ForEach-Object {
    Start-Job -ScriptBlock {
        Invoke-WebRequest -Uri http://localhost:8080/api/games -UseBasicParsing
    }
}

# Wait for all jobs to complete
$jobs | Wait-Job | Receive-Job
```

## Stopping the Environment

```powershell
# Stop all services (preserves data)
docker-compose down

# Stop and remove all data
docker-compose down -v
```

## Notes
- Both API instances share the same MongoDB database
- All instances use the same JWT secret for authentication
- Requests are automatically distributed by NGINX
- Each instance logs its name for easy tracking
- The setup uses Docker networking for internal communication
