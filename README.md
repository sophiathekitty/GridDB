# GridDB - Space Engineers Database Framework

A comprehensive framework for managing and displaying data in Space Engineers using text panels as storage and providing a rich GUI framework for interactive applications.

## Core Components

### GridDB System

The GridDB system provides distributed data storage using text panels as the storage medium, with support for networked data sharing between grids.

#### Key Classes

- **`GridDB`** - Core database class that manages data stored across text panels on a subgrid
  - Organizes data into domains and sub-categories (e.g., "Shows/Main", "Games/GameData")
  - Provides statistics and metrics (used/unused blocks, show/game counts)
  - Handles automatic storage allocation and data persistence

- **`GridData`** - Represents a collection of data blocks within a domain
  - Parses and manages header information and data blocks
  - Provides indexed access to data by name or position
  - Handles serialization/deserialization of stored data

- **`GridDataBlock`** - Individual data unit within a GridData collection
  - Stores name, type, and raw data content
  - Supports various data types and content formats

- **`GridDBAddress`** - Addressing system for locating specific data
  - Format: "domain:sub:index" (e.g., "Movies:Main:5")
  - Enables precise data retrieval across the database

#### Remote/Network Components

- **`GridDBServer`** - Provides network services for sharing data between grids
  - Responds to domain list requests, block lists, and data requests
  - Uses Space Engineers' Inter-Grid Communication (IGC) system
  
- **`GridDBClient`** - Client for accessing remote GridDB instances
  - Discovers available database hosts on the network
  - Downloads domain information and data from remote servers
  - Supports asynchronous data operations

- **`GridDBHostInfo`** - Information about remote database hosts
  - Tracks host ID, name, and available domains
  - Used for network discovery and connection management

### Screen Framework

A layered graphics framework for creating interactive displays on Space Engineers screens.

#### Core Screen Classes

- **`Screen`** - Base screen management class
  - Encapsulates an `IMyTextSurface` for drawing
  - Manages sprite layers and viewport calculations
  - Handles drawing frame management and sprite rendering

- **`ScreenSprite`** - Basic sprite implementation
  - Supports text, texture, and custom sprite types
  - Provides positioning with anchor points and alignments
  - Handles viewport transformations and collision detection

#### Advanced Sprite Types

- **`TextSprite`** - Multi-line text rendering with advanced formatting
- **`TextureSprite`** - Image/texture rendering with rotation and scaling
- **`RasterSprite`** - Pixel-perfect graphics using monospace font characters
  - Converts pixel coordinates to screen coordinates
  - Supports custom pixel-based graphics and colors
  - Used for detailed icons and low-resolution images

#### Interface System

- **`IScreenSprite`** - Base interface for all drawable objects
- **`IScreenSpriteProvider`** - Interface for objects that provide sprites to screens

### ScreenApp Framework

A GUI application framework built on top of the Screen system, providing structured layouts and user interaction.

#### Core App Classes

- **`ScreenApp`** - Base class for interactive screen applications
  - Extends Screen with input handling and application lifecycle
  - Manages game input, sound blocks, and app identification
  - Provides foundation for building complex user interfaces

- **`ScreenAppSeat`** - Manages application instances tied to specific game seats/controllers
  - Associates applications with physical pilot seats or cockpits
  - Handles app launching, switching, and lifecycle management
  - Supports multiple apps per seat with switching capabilities

#### Layout System (LayoutGUI)

- **`LayoutArea`** - Base container for organizing UI elements
  - Supports flexible width/height sizing
  - Manages margins, padding, and child element positioning
  - Provides automatic layout calculation and updates

- **`HorizontalLayoutArea`** - Horizontal arrangement of UI elements
- **`LayoutItem`** - Individual UI element with positioning and sizing

#### Interactive Components

- **`LayoutButton`** - Clickable button with text and icon support
  - Supports both text-only and icon+text configurations
  - Handles click events and visual feedback
  - Integrates with the interaction system

- **`LayoutMenu`** - Container for multiple interactive menu items
  - Automatically manages button layout and spacing
  - Supports dynamic menu item addition
  - Handles menu item selection and navigation

- **`InteractableGroup`** - Base class for managing groups of interactive elements
- **`IInteractable`** - Interface for elements that can receive input

#### Application Layout Components

- **`AppLayout`** - Complete application layout with header, content, and footer
  - Manages header with app title and file information
  - Provides content areas for main application functionality
  - Includes footer with key prompt information

- **`AppHeader`** - Application title bar showing app name and current file
- **`AppFooter`** - Bottom bar showing available keyboard shortcuts

### GridInfo System

A global state management and inter-script communication system.

#### Key Features

- **Variable Management** - Global variables with change notifications
- **Message Handling** - IGC message routing and processing
- **Script Communication** - Inter-programmable block communication
- **Grid Context** - Maintains references to grid terminal system, IGC, and programmable block

#### Core Classes

- **`GridInfo`** - Main state management class
  - Provides variable storage with change notifications
  - Handles IGC message routing and broadcasting
  - Manages script lifecycle and persistence

- **`GridBlocks`** - Block discovery and management utilities
  - Caches references to commonly used block types
  - Provides address-based block lookup for ScreenAppSeat
  - Handles automatic text panel naming and organization

- **`MessageData`** - Structured message format for inter-script communication
  - Key-value pair based message format
  - Support for client identification and addressing
  - Used throughout the remote database system

## Current Implementation Status

### Working Components

- ✅ **Core GridDB** - Fully functional database with local storage
- ✅ **Screen Framework** - Complete graphics system with sprite support
- ✅ **Basic ScreenApp** - Application framework with input handling
- ✅ **Layout System** - Functional UI layout and container system
- ✅ **Interactive Elements** - Buttons, menus, and basic interaction
- ✅ **GridInfo** - State management and communication system
- ✅ **Network Communication** - Basic client/server for data sharing

### Applications

- ✅ **DataManager** - Database browsing and management interface
- ✅ **WebBrowser** - Basic web-like content viewing system
- 🚧 **Advanced UI Components** - Scroll areas, complex layouts (partial)

### Framework State

The ScreenApp and GridDB frameworks are in a mature, functional state with most core features implemented. The system provides:

1. **Robust Data Storage** - Reliable text panel-based database with network sharing
2. **Rich Graphics** - Multi-layer sprite system with advanced rendering options
3. **Interactive GUI** - Complete application framework with layout management
4. **Modular Design** - Easy to extend with new applications and components
5. **Network Capabilities** - Distributed database access across multiple grids

The framework is suitable for building complex Space Engineers automation systems, HUD displays, and interactive control panels. The modular design allows developers to use individual components (Screen, GridDB, GridInfo) independently or as a complete application framework.

## Usage

Copy the scripts from the Common folder into your Space Engineers programmable block script to access the GridDB and ScreenApp frameworks. The system automatically initializes all components and provides a foundation for building custom applications.
