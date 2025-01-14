# Pdf_extractor



## Project Description

This Dynamics Tool is an application designed to automate the processing of PDF documents. The tool extracts relevant information from invoices (e.g., address details, invoice numbers) and transfers it to a predefined table within the Microsoft Power Platform. The goal is to reduce manual processes, increase efficiency, and minimize errors.

## Features

- Automated text extraction from PDF invoices
- Data storage in the Microsoft Dataverse environment
- Error and duplicate management with specific folders for unreadable or duplicate files
- Notifications for new records via Power Apps Flows
- Security mechanisms, including API key encryption (e.g., Azure Key Vault)
- Flexible configuration via external files

## Technologies

- **Programming Language**: C# (.NET Framework)
- **Tools and Frameworks**:
  - Microsoft Power Platform
  - Microsoft Power Apps
  - iText7 for PDF processing
  - Log4net for logging
  - Microsoft PowerPlatform.Dataverse.Client
  - Microsoft.Extensions.Configuration
- **Design Patterns**: Facade and Singleton

## System Requirements

- Windows operating system
- Visual Studio 2022
- Access to Microsoft Dataverse and Power Platform
- Configuration files for folder structures and API access data


