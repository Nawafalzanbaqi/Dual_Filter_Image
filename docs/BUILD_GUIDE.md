# 🔨 Build & Deployment Guide

**Complete instructions for building and deploying both editions**

---

## 📋 Table of Contents

1. [C# Edition Build](#c-edition-build)
2. [Java Edition Build](#java-edition-build)
3. [Deployment Options](#deployment-options)
4. [Troubleshooting](#troubleshooting)

---

## 🟦 C# Edition Build

### Prerequisites

**Required:**
- Windows 7 or later
- .NET 8.0 SDK (or Runtime for running only)

**Installation:**
```bash
# Download from: https://dotnet.microsoft.com/download
# Or use package manager:

# Windows (Chocolatey)
choco install dotnet-sdk

# Windows (Winget)
winget install Microsoft.DotNet.SDK.8

# macOS (Homebrew) - for cross-compilation only
brew install dotnet-sdk
```

### Build Steps

**1. Navigate to C# directory**
```bash
cd csharp
```

**2. Restore dependencies**
```bash
dotnet restore
```

**3. Build the project**
```bash
# Debug build
dotnet build

# Release build (optimized)
dotnet build -c Release
```

**4. Run the application**
```bash
# Development mode
dotnet run

# Release mode
dotnet run -c Release
```

### Deployment

**Option 1: Self-Contained Executable**
```bash
# Creates standalone executable with .NET runtime
dotnet publish -c Release -r win-x64 --self-contained

# Output: bin/Release/net8.0-windows/win-x64/publish/
# Size: ~100-150 MB
# Can run without .NET installed
```

**Option 2: Framework-Dependent**
```bash
# Creates executable that requires .NET runtime
dotnet publish -c Release -r win-x64

# Output: bin/Release/net8.0-windows/win-x64/publish/
# Size: ~5-10 MB
# Requires .NET 8.0 runtime installed
```

**Option 3: Development Distribution**
```bash
# Simple executable for development
dotnet build -c Release

# Run: bin/Release/net8.0-windows/ImageProcessorWinForms.exe
```

### Project Structure

```
csharp/
├── MainForm.cs                      # Main application (UI + Logic)
├── Program.cs                       # Entry point
├── ImageProcessorWinForms.csproj   # Project configuration
├── bin/                             # Build output
│   └── Release/
│       └── net8.0-windows/
└── obj/                             # Build artifacts
```

### Useful Commands

```bash
# Clean build artifacts
dotnet clean

# Verify project structure
dotnet list reference

# Check for outdated dependencies
dotnet outdated

# Format code
dotnet format

# Run tests (if added)
dotnet test
```

---

## 🟩 Java Edition Build

### Prerequisites

**Required:**
- Java 17 or later (JDK, not just JRE)
- Maven 3.6 or later

**Installation:**

**Java:**
```bash
# Download from: https://adoptium.net/
# Or use package manager:

# Windows (Chocolatey)
choco install openjdk

# Windows (Winget)
winget install EclipseAdoptium.Temurin.17

# macOS (Homebrew)
brew install openjdk@17

# Linux (Ubuntu/Debian)
sudo apt-get install openjdk-17-jdk

# Linux (Fedora/RHEL)
sudo dnf install java-17-openjdk-devel
```

**Maven:**
```bash
# Download from: https://maven.apache.org/download.cgi
# Or use package manager:

# Windows (Chocolatey)
choco install maven

# macOS (Homebrew)
brew install maven

# Linux (Ubuntu/Debian)
sudo apt-get install maven

# Linux (Fedora/RHEL)
sudo dnf install maven
```

### Build Steps

**1. Navigate to Java directory**
```bash
cd java
```

**2. Verify installation**
```bash
java -version
mvn -version
```

**3. Build the project**
```bash
# Clean and build
mvn clean package

# Skip tests (if any)
mvn clean package -DskipTests
```

**4. Run the application**
```bash
# Using Maven plugin
mvn javafx:run

# Or run JAR directly
java -jar target/ImageProcessorJavaFX-2.0.jar

# With verbose output
java -verbose:class -jar target/ImageProcessorJavaFX-2.0.jar
```

### Deployment

**Option 1: Executable JAR**
```bash
# Build JAR
mvn clean package

# Run JAR
java -jar target/ImageProcessorJavaFX-2.0.jar

# Distribution: Copy JAR to target system
# Requirements: Java 17+ installed
```

**Option 2: Self-Contained Package (Advanced)**
```bash
# Requires jpackage (included in Java 17+)
jpackage --input target \
         --name ImageProcessor \
         --main-jar ImageProcessorJavaFX-2.0.jar \
         --main-class com.imageprocessor.ImageProcessorApp \
         --type exe

# Creates native installer for Windows
```

### Project Structure

```
java/
├── pom.xml                          # Maven configuration
├── src/
│   └── main/
│       ├── java/
│       │   └── com/imageprocessor/
│       │       ├── ImageProcessorApp.java
│       │       ├── core/
│       │       │   ├── FilterType.java
│       │       │   └── ImageProcessor.java
│       │       └── ui/
│       │           └── MainWindow.java
│       └── resources/
│           └── styles.css
├── target/                          # Build output
│   ├── classes/                     # Compiled classes
│   ├── ImageProcessorJavaFX-2.0.jar # Executable JAR
│   └── ...
└── .classpath                       # IDE configuration
```

### Useful Commands

```bash
# Clean build artifacts
mvn clean

# Build without running
mvn compile

# Run tests
mvn test

# Generate documentation
mvn javadoc:javadoc

# Check dependencies
mvn dependency:tree

# Update dependencies
mvn versions:display-dependency-updates

# Format code
mvn spotless:apply
```

---

## 📦 Deployment Options

### Option 1: Direct Distribution

**C# Edition:**
```bash
# Create self-contained executable
cd csharp
dotnet publish -c Release -r win-x64 --self-contained

# Copy entire publish folder to users
# Users can run directly without installation
```

**Java Edition:**
```bash
# Create JAR
cd java
mvn clean package

# Copy JAR to users
# Users need Java 17+ installed
```

### Option 2: Installer Creation

**C# Edition:**
```bash
# Using WiX Toolset (advanced)
# Or use Visual Studio installer projects
# Creates MSI installer for Windows
```

**Java Edition:**
```bash
# Using jpackage
jpackage --input target \
         --name ImageProcessor \
         --main-jar ImageProcessorJavaFX-2.0.jar \
         --main-class com.imageprocessor.ImageProcessorApp \
         --type msi

# Creates MSI installer for Windows
```

### Option 3: Docker Containerization

**Java Edition (recommended):**
```dockerfile
FROM openjdk:17-slim

WORKDIR /app

COPY target/ImageProcessorJavaFX-2.0.jar .

ENTRYPOINT ["java", "-jar", "ImageProcessorJavaFX-2.0.jar"]
```

---

## 🔧 Troubleshooting

### C# Build Issues

**Issue: .NET SDK not found**
```bash
# Solution: Install .NET 8.0
dotnet --version
# If not found, download from https://dotnet.microsoft.com/download
```

**Issue: Build fails with "CS0103: The name does not exist"**
```bash
# Solution: Restore NuGet packages
dotnet restore
dotnet clean
dotnet build
```

**Issue: Application won't run**
```bash
# Solution: Check .NET runtime
dotnet --list-runtimes
# Ensure net8.0-windows is installed
```

### Java Build Issues

**Issue: Maven command not found**
```bash
# Solution: Install Maven
# Download from https://maven.apache.org/download.cgi
# Add to PATH environment variable
```

**Issue: "javac: command not found"**
```bash
# Solution: Install JDK (not JRE)
java -version
# Should show JDK version, not JRE
```

**Issue: Build fails with "ERROR: Could not find or load main class"**
```bash
# Solution: Ensure pom.xml is correct
mvn clean compile
mvn clean package
```

**Issue: JavaFX modules not found**
```bash
# Solution: Maven should download automatically
mvn clean install
# If not, check internet connection and Maven settings
```

### Runtime Issues

**C# - Application crashes on startup**
```bash
# Solution: Check Windows version
# Windows 7+ required
# Or check .NET runtime version
dotnet --list-runtimes
```

**Java - "No JavaFX runtime components are available"**
```bash
# Solution: Use Maven to run
mvn javafx:run
# Or ensure JavaFX is in classpath
```

---

## 📊 Build Performance

### C# Build Times
- **Clean Build**: ~5-10 seconds
- **Incremental Build**: ~1-2 seconds
- **Release Build**: ~10-15 seconds
- **Publish**: ~15-20 seconds

### Java Build Times
- **Clean Build**: ~10-15 seconds
- **Incremental Build**: ~2-3 seconds
- **Package**: ~15-20 seconds
- **JAR Creation**: ~5 seconds

---

## ✅ Verification Checklist

### C# Edition
- [ ] .NET 8.0 SDK installed
- [ ] Project builds without errors
- [ ] Application runs successfully
- [ ] All filters work correctly
- [ ] Image loading works
- [ ] File saving works
- [ ] No memory leaks

### Java Edition
- [ ] Java 17+ JDK installed
- [ ] Maven 3.6+ installed
- [ ] Project builds without errors
- [ ] Application runs successfully
- [ ] All filters work correctly
- [ ] Image loading works
- [ ] File saving works
- [ ] No memory leaks

---

## 🚀 Continuous Integration

### GitHub Actions Example (C#)

```yaml
name: C# Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      - run: cd csharp && dotnet build
      - run: cd csharp && dotnet test
```

### GitHub Actions Example (Java)

```yaml
name: Java Build

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-java@v2
        with:
          java-version: '17'
      - run: cd java && mvn clean package
```

---

## 📝 Release Checklist

- [ ] Update version numbers
- [ ] Update CHANGELOG
- [ ] Run full test suite
- [ ] Build release binaries
- [ ] Create release notes
- [ ] Tag release in Git
- [ ] Upload to release repository
- [ ] Announce release

---

## 🔗 Resources

### C# Resources
- [.NET Download](https://dotnet.microsoft.com/download)
- [dotnet CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [Windows Forms Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

### Java Resources
- [Java Download](https://adoptium.net/)
- [Maven Documentation](https://maven.apache.org/guides/)
- [JavaFX Documentation](https://openjfx.io/)

---

**Version**: 2.0  
**Last Updated**: 2025  
**Status**: ✅ Production Ready

---

**Happy Building! 🚀**
