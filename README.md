# Blocked-Countries API

A .NET Core API to manage blocked countries and validate IP addresses using third-party geolocation services (IPGeolocation.io). The application relies on in-memory data storage for managing blocked country lists.

## Features

- Block/unblock countries by country code
- Temporary country blocking with automatic expiration
- IP address geolocation lookup
- Check if an IP address is from a blocked country
- Logging of access attempts
- Pagination support for listing blocked countries

## Technologies

- .NET 8.0
- ASP.NET Core Web API
- Swagger/OpenAPI for API documentation
- IPGeolocation.io for IP geolocation services
- In-memory data storage using ConcurrentDictionary


### Prerequisites

- .NET 8.0 SDK or later
- IPGeolocation.io API key

### Configuration

1. Clone the repository
2. Open the `appsettings.json` file and replace the placeholder API key with your IPGeolocation.io API key:

```json
"GeoSettings": {
  "ApiKey": "Your-IPGeolocation-API-Key",
  "BaseUrl": "https://api.ipgeolocation.io/v2/ipgeo"
}
```

### Running the Application

```bash
cd BlockedCountriesApi
dotnet run
```

The API will be available at `https://localhost:5001` and `http://localhost:5000` by default.

## API Endpoints

### Countries Management

- **POST /api/countries/block** - Block a country permanently
  - Request body: `{ "countryCode": "US" }`

- **DELETE /api/countries/block/{countryCode}** - Unblock a country
  - URL parameter: `countryCode` (e.g., US, UK, CA)

- **POST /api/countries/temporal-block** - Block a country temporarily
  - Request body: `{ "countryCode": "US", "durationMinutes": 60 }`

- **GET /api/countries/blocked** - Get list of blocked countries
  - Query parameters: `page`, `pageSize`, `countryCode` (optional filter)

### IP Operations

- **GET /api/ip/lookup** - Lookup country information for an IP address
  - Query parameter: `ipAddress` (optional, uses client IP if not provided)

- **GET /api/ip/check-block** - Check if client IP is from a blocked country
  - Returns block status and logs the attempt

### Logs

- **GET /api/logs** - Get access attempt logs
  - Query parameters: `page`, `pageSize`, `countryCode` (optional filter)

## Background Services

The application includes a background service (`TemporalBlockCleanupService`) that periodically removes expired temporary blocks.

## Validation Rules

The API implements the following validation rules:

### Country Codes
- Must be exactly two uppercase English letters (e.g., US, UK, CA)
- Validated using regex pattern: `^[A-Z]{2}$`

### Temporal Block Duration
- Duration must be between 1 and 1440 minutes (24 hours)
- Validated using range validation: `[Range(1, 1440)]`

### Pagination Parameters
- Page number must be greater than or equal to 1
- Page size must be between 1 and 100 items per page


