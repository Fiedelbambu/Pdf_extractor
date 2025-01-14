# Daten aus einer PDF-Datei extrahieren

## Inhaltsverzeichnis
1. [Was soll gemacht werden?](#was-soll-gemacht-werden)
2. [Pdf-Dateien auslesen](#pdf-dateien-auslesen)
   - [PdfPig](#pdfpig)
   - [IronPDF](#ironpdf)
   - [PdfSharp](#pdfsharp)
   - [PDF Clown](#pdf-clown)
   - [iTextSharp/iText7](#itextsharpitext7)
3. [Probleme bei der Datenauswertung](#probleme-bei-der-datenauswertung)
4. [Verwendung eines Timers](#verwendung-eines-timers)
5. [Logger-Klasse](#logger-klasse)
6. [Mail-Versand](#mail-versand)
   - [Microsoft Graph API](#microsoft-graph-api)
   - [Microsoft Kiota](#microsoft-kiota)
   - [Office 365 REST API](#office-365-rest-api)
   - [Dynamics 365 Web API](#dynamics-365-web-api)
7. [Daten hinzufügen in Dynamics365](#daten-hinzufügen-in-dynamics365)
   - [Microsoft Graph](#microsoft-graph)
   - [Dynamics 365 Web API (Dataverse API)](#dynamics-365-web-api-dataverse-api)
   - [Microsoft.PowerPlatform.Dataverse.Client](#microsoftpowerplatformdataverseclient)
8. [Struktur - Aufbau der Klassenhierarchie](#struktur---aufbau-der-klassenhierarchie)
9. [Benutzerzugriff auf Dataverse](#benutzerzugriff-auf-dataverse)

## Was soll gemacht werden?

Diese Anwendung soll in einem ausgewählten Ordner nach Dateien mit der Endung `.pdf` suchen. Diese werden in einer Liste gespeichert (z.B. `Pdf_liste`). Der nächste Schritt wäre zu prüfen, ob diese Dateien gelesen werden können. Wenn ja, dann benötigen wir Daten aus dem Rechnungskopf wie Firmenname, Anschrift, Kundennummer und Ansprechpartner. Diese Daten sollen in einer Liste `extrahierteDaten` gespeichert werden.

Ist dieser Vorgang abgeschlossen, wird eine Verbindung zu einer Datenbank (MySQL, SQLite, ...) hergestellt. Benutzername, Passwort und Tabellenbezeichnung müssen stimmen. Wenn die Verbindung steht, soll geprüft werden, ob sich schon die gleichen Daten in der Tabelle befinden. Falls ja, werden die doppelten Daten aus der Liste `extrahierteDaten` entfernt und die restlichen Daten in der Tabelle neu angelegt.

## Pdf-Dateien auslesen

Zur Auswahl stehen:

### PdfPig
PdfPig ist eine Open-Source-Bibliothek, mit der Anwender PDFs in C# und anderen .NET-Sprachen lesen und erstellen können. Das letzte Update wurde am 05.06.2023 veröffentlicht.

### IronPDF
IronPDF ist eine vielseitige Bibliothek, jedoch nicht Open-Source und fällt daher schon mal heraus.

### PdfSharp
PdfSharp ist eine Open-Source-Bibliothek, die zuletzt am 13.08.2024 ein Update erhalten hat. Sie hat eine leicht zu bedienende API und ist plattformübergreifend verwendbar. Allerdings bietet sie keine direkte Konvertierung in andere Formate, weshalb dieses Paket auch ausgeschlossen wird.

### PDF Clown
Gut geeignet für kleinere Projekte, die grundlegende PDF-Manipulation und Textextraktion benötigen. Die LGPL-Lizenz ist ausreichend.

### iTextSharp/iText7
Ideal für professionelle und kommerzielle Anwendungen, die fortschrittliche PDF-Funktionalitäten benötigen, wie digitale Signaturen, umfassende Textextraktion, Formularmanipulation und mehr.

## Probleme bei der Datenauswertung

Probleme bei der Extraktion von Daten treten auf, wenn keine Standardisierung vorhanden ist. Beispielsweise kann "Firma: Hamsert" leicht ausgelesen werden. Aber wenn der Text nicht genau lokalisiert ist oder aus mehreren Zeilen besteht, wird es schwieriger, ein Muster zu erkennen.

Mit einem heuristischen Ansatz:
Durch die Kombination eines eingeschränkten Textbereichs und eines gut formulierten Regex-Musters kann man Adressen wie „Industriestrasse 10/12“ effizient extrahieren.

### Strategien in der iText-Bibliothek

- **SimpleTextExtractionStrategy**: Extrahiert Text ohne Berücksichtigung der genauen Position der Textblöcke.
- **FilteredTextEventListener**: Ermöglicht das Extrahieren von Text basierend auf bestimmten Filtern.
- **CustomTextExtractionStrategy**: Implementierung einer eigenen Strategie zur Textextraktion.
- **TextRenderInfo**: Liefert detaillierte Informationen über gerenderten Text.
- **LocationAwareTextExtractionStrategy**: Ermöglicht eine noch genauere Extraktion basierend auf Textpositionen.
- **GlyphTextEventListener**: Arbeitet auf einem niedrigeren Level der Glyphen.

Das **Strategy Design Pattern** ermöglicht es, verschiedene Algorithmen austauschbar zu verwenden.

## Verwendung eines Timers

Die Klassen `System.Timers.Timer` oder `System.Threading.Timer` können verwendet werden, ohne dass ein zusätzliches NuGet-Paket installiert werden muss.

- **System.Threading.Tasks**: Gut für asynchrone Aufgaben.
  
### Unterschiede zwischen `System.Threading` und `System.Timers`

- **Zweck**: `System.Threading` dient der asynchronen und parallelen Programmierung. `System.Timers` wird verwendet, um Aktionen in regelmäßigen Intervallen auszuführen.
- **Flexibilität**: `System.Threading` ist für verschiedene Arten von asynchronen Operationen geeignet, während `System.Timers` auf zeitgesteuerte, wiederholende Ausführungen spezialisiert ist.
- **Zeitsteuerung**: `System.Timers.Timer` ist präziser und ideal für regelmäßige, wiederkehrende Aufgaben.

## Logger-Klasse

Eine Logger-Klasse ist wichtig, um Informationen zu sammeln. Hierfür werden Standardklassen verwendet, die keine weitere Installation von NuGet-Paketen erfordern.

## Mail-Versand

Der Mail-Versand wird nicht im Code geschrieben. Es gibt in der Power Platform ein Feature, das hierfür verwendet werden soll. Mit CloudFlows lässt sich eine Benachrichtigung versenden, die über das angemeldete Mail-Konto läuft. Dazu ist eine kurze, selbsterklärende Plug&Play-Umgebung in der Power Platform vorhanden. Dies spart Zeit und reduziert den Code.

Zur Auswahl stehen:

#### Microsoft Graph API
Eine RESTful Web-API, die Zugriff auf Microsoft 365-Dienste bietet. Dienste wie E-Mails, Kalender, Kontakte, OneDrive, Teams und Dynamics 365 werden unterstützt. Diese API ist am besten für dieses Projekt geeignet. Hiermit kann ich auch eine Tabelle in den Dynamics-Diensten ansprechen, in die der Briefkopf geschrieben wird. Die Graph-API ist mit ihren umfassenden Funktionen und dem Graph-Explorer am besten für das Projekt geeignet.

Die Methode `SendMailPostRequestBody` ist ein essenzielles Element im Microsoft Graph SDK, wenn man programmtechnisch E-Mails versenden möchte. Sie strukturiert die E-Mail-Daten und ermöglicht, die Funktionalitäten der Microsoft Graph API zum Senden von E-Mails einfach zu nutzen.

#### Microsoft Kiota
Kiota ist ein Codegenerator, der API-Clients für verschiedene Programmiersprachen generiert, basierend auf OpenAPI-Spezifikationen. Es generiert den Client-Code für die Microsoft Graph API.

#### Office 365 REST API
Spezifisch für einzelne Features. Allerdings bevorzuge ich die Graph-API, da sie alle benötigten Funktionen abdeckt und nur ein Paket verwendet werden muss.

#### Dynamics 365 Web API
Beschränkt sich auf die Produkte von Dynamics 365.

## Daten hinzufügen in Dynamics365

Hier gibt es die gleichen Möglichkeiten wie beim Mail-Versand: Entweder mit den spezifischen APIs zu arbeiten oder die Graph-Klasse zu nutzen, die sehr umfangreich ist. Die Graph klasse ist nicht unbedingt geeignet für dieses Projekt

#### Dynamics 365 Web API (Dataverse API)
Stellt APIs bereit, um mit Daten in Dynamics 365 zu arbeiten, z.B. um CRM-Daten wie Kontakte, Accounts, Leads und benutzerdefinierte Entitäten zu verwalten. Dies ist die primäre API für die Arbeit mit Daten innerhalb von Dynamics 365.

#### Microsoft.PowerPlatform.Dataverse.Client
Der ServiceClient-Konstruktor in der Microsoft.PowerPlatform.Dataverse.Client-Bibliothek bietet verschiedene Möglichkeiten, eine Verbindung zur Dataverse-Umgebung herzustellen.


## Benutzerzugriff auf Dataverse

Wenn ein Benutzer, der nicht Teil der Microsoft Entra ID ist, Zugriff auf die Dataverse-Organisation benötigt, muss der Benutzer dem Mandanten als externer Benutzer oder Gastbenutzer hinzugefügt werden. Ausführliche Schritte finden Sie unter „Benutzer für B2B-Zusammenarbeit im Microsoft Entra Admin Center hinzufügen“.

## DPAPI Verwenden zum Verschlüsseln der Verbindung zu PowerPages
DPAPI arbeitet in zwei Modi. Benutzermodus  (DataProtectionScope.CurrentUser) Die Daten sind nur für den aktuellen Benutzer zugänglich, der sie verschlüsselt hat. Computermodus (DataProtectionScope.LocalMachine) Die Daten sind alle für alle Benutzer auf dem gleichen Computer zugänglich. DPAPI erfordert kein komplexes Schlüsselmanagement, da die Anmeldeinformationen verwendet werden. Die Verschlüsselung ist durch das Anmeldesystem geschützt und die Daten können nur entschlüsselt werden, wenn der authentifiziert ist. Kein explizieter Schlüssel erforderlich, da DPAPI die Anmeldeinformationen verwendet, muss du dich nicht um die Speicherung oder Verwaltung eines Schlüssel kümmern. Sicherheitsmerkmale Bindung an Benutzer und Computer, keine externen Schlüsselspeicherung<br>
### Vorteile von DPAPI
kostenlos DPAPI ist ein Teil von Windows und erfordert keine zusätzlichen kosten. Integriert in Windows und Einfacher Code
### Nachteile
Nur für Windows geeignet, Gebunden an Windows Anmeldeinformationen

## Struktur - Aufbau der Klassenhierarchie

PDFProcessingApp        
│<br>
└── Program.cs (Main entry point)<br>
│<br>
├── Commands<br> 
│<br>
├── ReadPdfCommand.cs<br> 
│<br>
├── ArchiveDataCommand.cs<br> 
│<br>
└── PowerPlatform.cs<br> 
│<br>
├── Scheduler<br> 
│<br>
└── SchedulerService.cs<br> 
│<br>
├── Services<br> 
│ <br>
├── PdfReaderService.cs<br>
│<br> 
└── ArchiverService.cs <br>



##DPAPI (Data Protection API) <br>
ist eine Windows-eigene API, die es Anwendungen ermöglicht, sensible Daten wie Passwörter, Schlüssel oder Zugangsdaten sicher zu verschlüsseln und wieder zu entschlüsseln. Sie wurde entwickelt, um diese Aufgaben ohne den Aufwand eines kompletten Verschlüsselungssystems zu erledigen. DPAPI verwendet unter der Haube die Windows-Anmeldeinformationen des Benutzers oder Systems, um Daten zu schützen, sodass nur der jeweilige Benutzer oder das System die Daten entschlüsseln kann.