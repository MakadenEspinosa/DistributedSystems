const express = require('express');
const app = express();
const port = 3007;

const mongoConntectionString = 'mongodb://localhost:27017/super_cool_api_db';

app.use(express.json());