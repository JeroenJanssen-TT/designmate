# ReDesignMate

**ReDesignMate** is a Grasshopper plugin for generative design and performance analysis of building renovations. It leverages Rhino's Hops and a local compute server to enable web-based interaction with parametric building models.

## 🔧 Features

- Generates building floor geometry using configurable design parameters
- Supports different facade systems with structural and carbon footprint analysis
- Integrates seamlessly with web interfaces through Hops and compute server
- Encapsulates modular Grasshopper components for flexible pipeline setup

---

## Getting Started

To run ReDesignMate locally, follow these steps:

### 1. Prerequisites

- **Rhino**
- **Grasshopper with Hops plugin**  
  (Ensure Hops is installed via PackageManager)
- **Node.js** and **VS Code** for running the web interface

### 2. Install & Launch

1. **Install the ReDesignMate Grasshopper plugin**  
   Copy the compiled plugin files (`.gha`, `.dll`) into your Grasshopper Libraries folder.

2. **Run the Rhino Compute Server**  
   Launch from this path:
   ```bash
   "%AppData%\Roaming\McNeel\Rhinoceros\packages\8.0\Hops\0.16.23\compute.geometry\compute.geometry.exe"

3. **Start the Web Interface**

  Open the compute-rhino3d-appserver folder in VS Code

Run the app:

bash
Copy code
npm install
npm start
Explore in Browser

Visit: http://localhost:3000/example/
