# ImageToPdfAPI

ImageToPdfAPI is a .NET Core Web API service that allows users to convert various image and document formats to PDF. The API supports TIFF, DOCX, MSG, and other image formats like jpeg, png etc.

## Table of Contents

- [Features](#Features)
- [Technologies-Used](#Technologies-Used)
- [Prerequisites](#Prerequisites)
- [Installation-of-Nugget-Packages](#Installation-of-Nugget-Packages)
- [Setup-and-Usage](#Setup-and-Usage)
- [Contributions](#contributions)

## Features

- Convert TIFF images to PDF.
- Convert DOCX images to PDF.
- Convert MSG files images to PDF.
- Supports other image formats like jpeg, png etc for conversion.

## Technologies-Used

- C#
- ASP.NET Core
- Swashbuckle.AspNetCore
- PdfSharp library
- MsgReader
- Docx (by Xceed)
- BitMiracle.LibTiff.Net


### Prerequisites

Visual Studio,.NET SDK installed on your machine(.NET, .NET Core), Different types of Images(.tiff, .docx, .msg, .png, .jpeg), NuGet Package Manager. 

### Installation-of-Nugget-Packages
Ensure the presence of the following NuGet packages in your project. If any of these packages are missing, install them using the Package Manager Console or Visual Studio NuGet Package Manager.

* PdfSharp library -> Install-Package PdfSharp
* MsgReader -> Install-Package MsgReader
* Docx (by Xceed) -> Install-Package Xceed.Words.NET
* BitMiracle.LibTiff.Net -> Install-Package BitMiracle.LibTiff.Net


### Setup-and-Usage

1. Clone the repository:

   ```bash
   git clone https://github.com/morabeeh/ImageToPdfAPI.git

2. Navigate to the Project Directory and open the Solution

3. Build and run the application on your ASP.NET Core environment

4. Check Whether Swagger is running perfectly with the localhost

5. Navigate to the /api/ImageToPdf/Convert and upload the Image and Execute.

7. Check the Response whether its a success or not

6. Locate the Downloaded Pdf location from the response and open Pdf from there.


### Contributions
Contributions are welcome! If you find a bug or have a feature request, please open an issue or submit a pull request.
