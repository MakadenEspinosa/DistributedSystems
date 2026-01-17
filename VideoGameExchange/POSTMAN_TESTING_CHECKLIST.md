# Video Game Exchange API - Postman Testing Checklist

## Base URL
`http://localhost:5171`

---

## 🔐 Authentication Endpoints

### ☐ POST /auth/login
**Purpose:** Get JWT token for authenticated requests

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

**Expected:** 200 OK with JWT token
**Save the token:** Use it in Authorization header as `Bearer {token}` for protected endpoints

---

## 👥 User Endpoints

### ☐ GET /users
**Purpose:** Retrieve all users (public endpoint)

**Expected:** 200 OK with array of users and HATEOAS links

**Check for:**
- `items` array containing user resources
- `_links` array with collection links (self, create)
- Each user has `links` array with available actions

---

### ☐ POST /users (Create User #1)
**Purpose:** Create first user account

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "streetAddress": "123 Main St, Springfield, IL 62701"
}
```

**Expected:** 201 Created
**Check for:**
- `Location` header with new user URL
- Response body with user resource including `id` and `links`
- User should NOT contain password in response

**Save:** User ID for later tests

---

### ☐ POST /users (Create User #2)
**Purpose:** Create second user account

**Request Body:**
```json
{
  "name": "Jane Smith",
  "email": "jane.smith@example.com",
  "password": "MyPassword456!",
  "streetAddress": "456 Oak Avenue, Portland, OR 97205"
}
```

**Expected:** 201 Created
**Save:** User ID for later tests

---

### ☐ POST /users (Create User #3)
**Purpose:** Create third user for testing

**Request Body:**
```json
{
  "name": "Mike Johnson",
  "email": "mike.johnson@example.com",
  "password": "GamePass789!",
  "streetAddress": "789 Elm Street, Austin, TX 78701"
}
```

**Expected:** 201 Created
**Save:** User ID for later tests

---

### ☐ POST /users (Duplicate Email Test)
**Purpose:** Verify email uniqueness constraint

**Request Body:**
```json
{
  "name": "John Duplicate",
  "email": "john.doe@example.com",
  "password": "AnotherPass!",
  "streetAddress": "999 Test St"
}
```

**Expected:** 400 Bad Request
**Check for:** Error message about duplicate email

---

### ☐ GET /users/{userid}
**Purpose:** Get specific user details

**Use:** User ID from previous POST
**Expected:** 200 OK with user details and HATEOAS links

---

### ☐ PATCH /users/{userid}
**Purpose:** Partially update user (name and/or address)

**Request Body:**
```json
{
  "name": "Jonathan Doe",
  "streetAddress": "123 Main St, Apt 4B, Springfield, IL 62701"
}
```

**Expected:** 200 OK with updated user resource
**Check:** Name and address are updated, email unchanged

---

### ☐ PATCH /users/{userid} (Update Password)
**Purpose:** Change user password

**Request Body:**
```json
{
  "password": "NewSecurePass999!"
}
```

**Expected:** 200 OK
**Test:** Login with new password afterwards

---

### ☐ DELETE /users/{userid}
**Purpose:** Delete a user account

**Expected:** 204 No Content
**Verify:** GET request to same user returns 404

---

## 🎮 Game Endpoints

### ☐ GET /games
**Purpose:** Get all games (public endpoint)

**Expected:** 200 OK with games array and HATEOAS links

---

### ☐ GET /games?userid={userid}
**Purpose:** Filter games by user

**Expected:** 200 OK with games only from specified user

---

### ☐ POST /games (Game #1 - Complete Data)
**Purpose:** Add game with all fields

**Request Body:**
```json
{
  "title": "The Legend of Zelda: Breath of the Wild",
  "platform": "Switch",
  "condition": "Excellent",
  "year": 2017,
  "publisher": "Nintendo",
  "userId": "{use-john-doe-user-id}"
}
```

**Expected:** 201 Created
**Check for:**
- `Location` header
- Response with `id` and `links`

**Save:** Game ID for later tests

---

### ☐ POST /games (Game #2 - Minimal Required)
**Purpose:** Add game with only required fields

**Request Body:**
```json
{
  "title": "Halo Infinite",
  "platform": "Xbox",
  "userId": "{use-john-doe-user-id}"
}
```

**Expected:** 201 Created
**Check:** Optional fields are null or default values

---

### ☐ POST /games (Game #3)
**Purpose:** Add game for second user

**Request Body:**
```json
{
  "title": "God of War Ragnarök",
  "platform": "PS5",
  "condition": "Mint",
  "year": 2022,
  "publisher": "Sony Interactive Entertainment",
  "userId": "{use-jane-smith-user-id}"
}
```

**Expected:** 201 Created

---

### ☐ POST /games (Game #4)
**Purpose:** Add another game for Jane

**Request Body:**
```json
{
  "title": "Spider-Man 2",
  "platform": "PS5",
  "condition": "Good",
  "year": 2023,
  "publisher": "Sony Interactive Entertainment",
  "userId": "{use-jane-smith-user-id}"
}
```

**Expected:** 201 Created

---

### ☐ POST /games (Game #5)
**Purpose:** Add PC game

**Request Body:**
```json
{
  "title": "Cyberpunk 2077",
  "platform": "PC",
  "condition": "Fair",
  "year": 2020,
  "publisher": "CD Projekt Red",
  "userId": "{use-mike-johnson-user-id}"
}
```

**Expected:** 201 Created

---

### ☐ POST /games (Invalid Year Test)
**Purpose:** Test validation for year range

**Request Body:**
```json
{
  "title": "Invalid Game",
  "platform": "PC",
  "year": 1950,
  "userId": "{user-id}"
}
```

**Expected:** 400 Bad Request (year must be >= 1970)

---

### ☐ GET /games/{gameid}
**Purpose:** Get specific game details

**Use:** Game ID from previous POST
**Expected:** 200 OK with game details and HATEOAS links

---

### ☐ PUT /games/{gameid}
**Purpose:** Full replacement of game data

**Request Body:**
```json
{
  "title": "The Legend of Zelda: Breath of the Wild - Special Edition",
  "platform": "Switch",
  "condition": "Mint",
  "year": 2017,
  "publisher": "Nintendo",
  "userId": "{use-john-doe-user-id}"
}
```

**Expected:** 200 OK with updated game
**Check:** All fields are replaced (not merged)

---

### ☐ PATCH /games/{gameid} (Update Condition)
**Purpose:** Partially update game condition

**Headers:** 
- `Content-Type: application/merge-patch+json`

**Request Body:**
```json
{
  "condition": "Good"
}
```

**Expected:** 200 OK
**Check:** Only condition changed, other fields unchanged

---

### ☐ PATCH /games/{gameid} (Multiple Fields)
**Purpose:** Update multiple fields at once

**Headers:** 
- `Content-Type: application/merge-patch+json`

**Request Body:**
```json
{
  "condition": "Fair",
  "publisher": "Nintendo of America"
}
```

**Expected:** 200 OK
**Check:** Both fields updated

---

### ☐ PATCH /games/{gameid} (Set Field to Null)
**Purpose:** Remove optional field value

**Headers:** 
- `Content-Type: application/merge-patch+json`

**Request Body:**
```json
{
  "publisher": null
}
```

**Expected:** 200 OK
**Check:** Publisher field is now null

---

### ☐ DELETE /games/{gameid}
**Purpose:** Delete a game from collection

**Expected:** 204 No Content
**Verify:** GET request to same game returns 404

---

## 🔗 HATEOAS Testing

### ☐ Verify All Resources Have Links
**Check each response contains:**
- `_links` or `links` array
- Links include `href`, `rel`, and `method`
- Common relations: `self`, `update`, `delete`, `collection`

---

### ☐ Follow Links Dynamically
**Purpose:** Test hypermedia navigation

1. GET /users
2. Use a link from response to get specific user
3. Use update link to PATCH the user
4. Use delete link to DELETE the user

---

## 🔒 Authorization Testing

### ☐ POST /games Without Token
**Purpose:** Verify authentication required

**Expected:** 401 Unauthorized

---

### ☐ PATCH /users Without Token
**Purpose:** Verify authentication required

**Expected:** 401 Unauthorized

---

### ☐ DELETE /games Without Token
**Purpose:** Verify authentication required

**Expected:** 401 Unauthorized

---

## 🧪 Edge Cases & Error Handling

### ☐ GET /users/{invalid-id}
**Expected:** 404 Not Found with Problem details

---

### ☐ GET /games/{invalid-id}
**Expected:** 404 Not Found with Problem details

---

### ☐ POST /users (Missing Required Field)
**Request Body:**
```json
{
  "name": "Test User",
  "email": "test@example.com"
}
```

**Expected:** 400 Bad Request (missing password and streetAddress)

---

### ☐ POST /games (Missing Required Field)
**Request Body:**
```json
{
  "title": "Test Game",
  "userId": "{user-id}"
}
```

**Expected:** 400 Bad Request (missing platform)

---

### ☐ POST /users (Invalid Email Format)
**Request Body:**
```json
{
  "name": "Test User",
  "email": "not-an-email",
  "password": "pass123",
  "streetAddress": "123 Test St"
}
```

**Expected:** 400 Bad Request

---

## 📊 Summary Checklist

- [ ] All users created successfully
- [ ] All games created successfully
- [ ] All GET endpoints return proper data structure
- [ ] All POST endpoints create resources and return 201
- [ ] All PUT endpoints fully replace resources
- [ ] All PATCH endpoints partially update resources
- [ ] All DELETE endpoints remove resources (204)
- [ ] All error cases return proper status codes and Problem details
- [ ] HATEOAS links present in all responses
- [ ] Authentication works and protects endpoints
- [ ] Validation prevents invalid data
- [ ] Collection filtering works (games by userid)

---

## 💡 Testing Tips

1. **Save IDs:** Keep track of created user and game IDs for subsequent tests
2. **Bearer Token:** After login, add token to Postman's Authorization tab
3. **Content-Type:** Use `application/merge-patch+json` for PATCH requests
4. **Check Status Codes:** Verify exact status codes match expected values
5. **Validate HATEOAS:** Every resource should have navigable links
6. **Problem Details:** All errors should return RFC 7807 Problem format
7. **Test Order:** Create users first, then games (games need userId)
