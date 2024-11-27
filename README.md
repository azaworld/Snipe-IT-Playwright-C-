# Snipe-IT-Playwright-C-

To install, clone, and run the project using Playwright and NUnit, follow these steps:

### Prerequisites:
1. **Install .NET SDK**: Make sure you have the .NET SDK installed on your machine. You can download and install it from the official Microsoft website: [Download .NET](https://dotnet.microsoft.com/download/dotnet).
   
2. **Install Node.js**: Playwright requires Node.js to run. Install Node.js from the official site: [Download Node.js](https://nodejs.org/).

### Step 1: Clone the Repository

1. **Clone the Git repository**: Open a terminal or command prompt and use the following command to clone the project repository from GitHub or another source:
   ```bash
   git clone <repository-url>
   ```
   Replace `<repository-url>` with the URL of your Git repository.

2. **Navigate to the project directory**:
   ```bash
   cd <project-directory>
   ```

### Step 2: Install Dependencies

1. **Install .NET dependencies**:
   Run the following command to restore all the NuGet packages required for the project:
   ```bash
   dotnet restore
   ```

2. **Install Playwright**:
   After the `.NET` dependencies are restored, you need to install Playwright:
   ```bash
   dotnet add package Microsoft.Playwright
   ```
   
3. **Install NUnit dependencies**:
   If not already added, you can install the NUnit testing framework by running:
   ```bash
   dotnet add package NUnit
   ```

4. **Install Playwright browsers**:
   You will need to install the necessary browsers (Chromium, Firefox, WebKit) for Playwright to function properly. Run the following command:
   ```bash
   playwright install
   ```

### Step 3: Run the Project

1. **Build the project**:
   Once the dependencies are installed, build the project using the following command:
   ```bash
   dotnet build
   ```

2. **Run the tests**:
   To run the tests, use the following command:
   ```bash
   dotnet test
   ```

   This will execute all tests written in the project using NUnit. The Playwright tests will run using the browser instances installed by Playwright.

### Step 4: View Test Results

- The results of your test execution will be shown in the console.
- If you want to view detailed reports, you can configure NUnit to output results to an XML or other format. For example:
  ```bash
  dotnet test --logger "nunit;LogFilePath=test-results.xml"
  ```
