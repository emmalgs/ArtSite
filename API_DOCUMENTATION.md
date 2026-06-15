# ArtSite API Documentation

Base URL: `https://artsite-5uh2.onrender.com`

## Table of Contents
- [Authentication](#authentication)
- [Artworks](#artworks)
- [Locations](#locations)
- [Shows](#shows)
- [Artist Info](#artist-info)

---

## Authentication

### Login
```http
POST /api/auth/login
```

**Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-12-31T23:59:59Z"
}
```

**Status Codes:**
- `200 OK` - Login successful
- `401 Unauthorized` - Invalid credentials

---

## Artworks

### Get All Artworks
```http
GET /api/artwork
```

**Response:**
```json
[
  {
    "artWorkId": 1,
    "title": "Sunset over Mountains",
    "medium": "Oil on Canvas",
    "category": "Landscape",
    "dimensions": "24x36 inches",
    "year": 2023,
    "price": 1500.00,
    "available": true,
    "primaryImageUrl": "https://storage.url/image.jpg"
  }
]
```

### Get Artwork by ID
```http
GET /api/artwork/{id}
```

**Response:**
```json
{
  "artWorkId": 1,
  "title": "Sunset over Mountains",
  "medium": "Oil on Canvas",
  "category": "Landscape",
  "dimensions": "24x36 inches",
  "year": 2023,
  "price": 1500.00,
  "available": true,
  "location": {
    "locationId": 1,
    "name": "Museum of Modern Art",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "url": "https://moma.org",
    "email": "info@moma.org",
    "phone": "212-555-1234",
    "collectionType": "Museum"
  },
  "images": [
    {
      "artworkImageId": 1,
      "imageUrl": "https://storage.url/image1.jpg",
      "altText": "Front view",
      "isPrimary": true
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - Artwork not found

### Create Artwork
```http
POST /api/artwork
```
**Requires Authentication**

**Request Body:**
```json
{
  "title": "Sunset over Mountains",
  "medium": "Oil on Canvas",
  "category": "Landscape",
  "dimensions": "24x36 inches",
  "year": 2023,
  "price": 1500.00,
  "available": true,
  "locationId": 1  // optional
}
```

**Response:**
```json
{
  "artWorkId": 1,
  "title": "Sunset over Mountains",
  // ... full artwork details
}
```

**Status Codes:**
- `201 Created` - Artwork created successfully
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - Not authenticated

### Update Artwork
```http
PUT /api/artwork/{id}
```
**Requires Authentication**

**Request Body:** (same as Create)

**Status Codes:**
- `200 OK` - Artwork updated
- `404 Not Found` - Artwork not found
- `401 Unauthorized` - Not authenticated

### Delete Artwork
```http
DELETE /api/artwork/{id}
```
**Requires Authentication**

**Status Codes:**
- `204 No Content` - Artwork deleted
- `404 Not Found` - Artwork not found
- `401 Unauthorized` - Not authenticated

### Upload Artwork Image
```http
POST /api/artwork/{artworkId}/images
```
**Requires Authentication**

**Request:** `multipart/form-data`
- `image` (file) - Image file (JPEG, PNG, WebP)
- `altText` (string, optional) - Alt text for accessibility
- `isPrimary` (boolean, optional) - Set as primary image

**Response:**
```json
{
  "artworkImageId": 1,
  "imageUrl": "https://storage.url/image.jpg",
  "altText": "Front view",
  "isPrimary": true
}
```

**Status Codes:**
- `200 OK` - Image uploaded
- `400 Bad Request` - Invalid file type
- `404 Not Found` - Artwork not found
- `401 Unauthorized` - Not authenticated

### Delete Artwork Image
```http
DELETE /api/artwork/{artworkId}/images/{imageId}
```
**Requires Authentication**

**Status Codes:**
- `204 No Content` - Image deleted
- `404 Not Found` - Image not found
- `401 Unauthorized` - Not authenticated

---

## Locations

### Get All Locations
```http
GET /api/location
```

**Response:**
```json
[
  {
    "locationId": 1,
    "name": "Museum of Modern Art",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "url": "https://moma.org",
    "email": "info@moma.org",
    "phone": "212-555-1234",
    "collectionType": "Museum"
  }
]
```

### Get Location by ID
```http
GET /api/location/{id}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - Location not found

### Create Location
```http
POST /api/location
```
**Requires Authentication**

**Request Body:**
```json
{
  "name": "Museum of Modern Art",
  "city": "New York",
  "state": "NY",
  "country": "USA",
  "url": "https://moma.org",
  "email": "info@moma.org",
  "phone": "212-555-1234",
  "collectionType": "Museum"
}
```

**Status Codes:**
- `201 Created` - Location created
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - Not authenticated

### Update Location
```http
PUT /api/location/{id}
```
**Requires Authentication**

**Request Body:** (same as Create)

**Status Codes:**
- `200 OK` - Location updated
- `404 Not Found` - Location not found
- `401 Unauthorized` - Not authenticated

### Delete Location
```http
DELETE /api/location/{id}
```
**Requires Authentication**

**Status Codes:**
- `204 No Content` - Location deleted
- `404 Not Found` - Location not found
- `401 Unauthorized` - Not authenticated

---

## Shows

### Get All Shows
```http
GET /api/show
```

**Response:**
```json
[
  {
    "showId": 1,
    "title": "Summer Exhibition 2024",
    "dates": "June 1 - August 31, 2024",
    "showType": "Group",
    "locationId": 1,
    "locationName": "Museum of Modern Art"
  }
]
```

### Get Show by ID
```http
GET /api/show/{id}
```

**Response:**
```json
{
  "showId": 1,
  "title": "Summer Exhibition 2024",
  "dates": "June 1 - August 31, 2024",
  "showInfo": "Annual summer exhibition featuring contemporary artists",
  "showType": "Group",
  "artistStatement": "This exhibition explores...",
  "location": {
    "locationId": 1,
    "name": "Museum of Modern Art",
    "city": "New York",
    "state": "NY",
    // ... full location details
  },
  "artworks": [
    {
      "artWorkId": 1,
      "title": "Sunset over Mountains",
      "medium": "Oil on Canvas",
      "year": 2023,
      // ... artwork details
    }
  ],
  "images": [
    {
      "showImageId": 1,
      "imageUrl": "https://storage.url/postcard.jpg",
      "altText": "Exhibition postcard",
      "imageType": "Postcard"
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - Show not found

### Create Show
```http
POST /api/show
```
**Requires Authentication**

**Request Body:**
```json
{
  "title": "Summer Exhibition 2024",
  "dates": "June 1 - August 31, 2024",
  "showInfo": "Annual summer exhibition featuring contemporary artists",
  "showType": "Group",
  "artistStatement": "This exhibition explores...",
  "locationId": 1,
  "artworkIds": [1, 2, 3]  // optional array of artwork IDs to include
}
```

**Status Codes:**
- `201 Created` - Show created
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - Not authenticated

### Update Show
```http
PUT /api/show/{id}
```
**Requires Authentication**

**Request Body:** (same as Create)

**Status Codes:**
- `200 OK` - Show updated
- `404 Not Found` - Show not found
- `401 Unauthorized` - Not authenticated

### Delete Show
```http
DELETE /api/show/{id}
```
**Requires Authentication**

**Status Codes:**
- `204 No Content` - Show deleted
- `404 Not Found` - Show not found
- `401 Unauthorized` - Not authenticated

### Upload Show Image
```http
POST /api/show/{showId}/images
```
**Requires Authentication**

**Request:** `multipart/form-data`
- `image` (file) - Image file (JPEG, PNG, WebP)
- `altText` (string, optional) - Alt text
- `imageType` (string, optional) - Type: "Postcard", "Installation", "Event", "Other"

**Response:**
```json
{
  "showImageId": 1,
  "imageUrl": "https://storage.url/image.jpg",
  "altText": "Exhibition postcard",
  "imageType": "Postcard"
}
```

**Status Codes:**
- `200 OK` - Image uploaded
- `400 Bad Request` - Invalid file
- `404 Not Found` - Show not found
- `401 Unauthorized` - Not authenticated

### Delete Show Image
```http
DELETE /api/show/{showId}/images/{imageId}
```
**Requires Authentication**

**Status Codes:**
- `204 No Content` - Image deleted
- `404 Not Found` - Image not found
- `401 Unauthorized` - Not authenticated

---

## Artist Info

### Get Current Artist Info
```http
GET /api/artistinfo
```

**Response:**
```json
{
  "artistInfoId": 1,
  "cv": "Education:\n- MFA, School of Visual Arts, 2018\n...",
  "bio": "Artist bio text here...",
  "artistStatement": "My work explores...",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-06-14T12:00:00Z"
}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - No artist info exists yet

### Create or Update Artist Info
```http
PUT /api/artistinfo
```
**Requires Authentication**

**Request Body:**
```json
{
  "cv": "Education:\n- MFA, School of Visual Arts, 2018\n...",
  "bio": "Artist bio text here...",
  "artistStatement": "My work explores...",
  "changeDescription": "Updated education section"
}
```

**Response:**
```json
{
  "artistInfoId": 1,
  "cv": "...",
  "bio": "...",
  "artistStatement": "...",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-06-14T12:00:00Z"
}
```

**Notes:**
- Automatically creates a version history entry
- If no artist info exists, creates a new one
- If exists, updates and saves previous version

**Status Codes:**
- `200 OK` - Updated successfully
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - Not authenticated

### Get Version History
```http
GET /api/artistinfo/versions
```

**Response:**
```json
[
  {
    "artistInfoVersionId": 5,
    "cv": "...",
    "bio": "...",
    "artistStatement": "...",
    "versionCreatedAt": "2024-06-14T12:00:00Z",
    "changeDescription": "Updated education section"
  },
  {
    "artistInfoVersionId": 4,
    "cv": "...",
    "bio": "...",
    "artistStatement": "...",
    "versionCreatedAt": "2024-05-01T10:00:00Z",
    "changeDescription": "Added new exhibition"
  }
]
```

### Get Specific Version
```http
GET /api/artistinfo/versions/{versionId}
```

**Status Codes:**
- `200 OK` - Success
- `404 Not Found` - Version not found

### Restore Version
```http
POST /api/artistinfo/restore/{versionId}
```
**Requires Authentication**

**Notes:**
- Restores artist info to a previous version
- Automatically creates a backup of current state before restoring

**Response:**
```json
{
  "artistInfoId": 1,
  "cv": "...",
  "bio": "...",
  "artistStatement": "...",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-06-14T15:00:00Z"
}
```

**Status Codes:**
- `200 OK` - Version restored
- `404 Not Found` - Version not found
- `401 Unauthorized` - Not authenticated

---

## Common Response Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `204 No Content` - Request successful, no content to return
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required or invalid token
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Authentication Header

For protected endpoints, include the JWT token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Notes

- All timestamps are in UTC
- Image uploads are processed and stored in Supabase Storage
- Images are automatically converted to JPEG format
- Maximum image upload size: 10MB
- Free tier on Render: App sleeps after 15 minutes of inactivity (30 second wake-up time on first request)
