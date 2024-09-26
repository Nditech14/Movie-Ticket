# Movie Ticket App

The Movie Ticket App is a backend service for managing movie tickets and movie-related data, including actors and their images. Users can browse available movies, add them to a cart, search by title, and perform other operations. The app integrates with Cosmos DB for data persistence and includes user authentication, session management, cart functionality, and media uploads.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture](#architecture)
- [Setup and Installation](#setup-and-installation)
- [API Endpoints](#api-endpoints)
- [MovieService Flow](#movieservice-flow)
- [Error Handling](#error-handling)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Movie Management**: Add, delete, and search for movies by title.
- **Actor Management**: Add, update, and delete actors, and upload actor images.
- **Cart Management**: Users can add or remove movies from their cart.
- **Session Management**: Cart items are preserved in the session for the duration of a user's activity.
- **Movie Expiry Check**: Expired movies cannot be added to the cart.
- **Cosmos DB Integration**: Uses Cosmos DB for storing cart, movie, and actor data.
- **File Upload**: Users can upload images for movies and actors.
- **Pagination**: Implements the "load more" functionality for movies and actors.
- **User Authentication**: Each cart is associated with a specific user.
- **Exception Handling**: Well-defined error handling with descriptive messages and HTTP status codes.

## Technologies Used

- **.NET Core 8**: Backend framework for building the API.
- **Cosmos DB**: For managing and storing data.
- **Azure Blob Storage**: For storing and serving uploaded movie and actor images.
- **AutoMapper**: To map entities and DTOs.
- **ASP.NET Core Web API**: To handle RESTful API requests.
- **Dependency Injection**: For managing services and repositories.
- **Session Management**: For user session handling using `IHttpContextAccessor`.
- **Swagger**: API documentation.

## Architecture

The application follows a layered architecture to separate concerns:

1. **Core**: Contains entities (`Cart`, `Movie`, `Actor`, `CartItem`), enums, and business logic.
2. **Application**: Manages services (`MovieService`, `CartService`, `ActorService`) and data transfer objects (DTOs). The services handle business logic and interact with the infrastructure.
3. **Infrastructure**: Handles interaction with Cosmos DB and Azure Blob Storage.
4. **Presentation**: Exposes REST API endpoints for interacting with the system. Controllers interact with services and return HTTP responses.

## Setup and Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8)
- [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) for data persistence
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/) for image uploads
- (Optional) [Azure Active Directory](https://azure.microsoft.com/en-us/services/active-directory/) for authentication

### Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/movie-ticket-app.git
   cd movie-ticket-app

 
