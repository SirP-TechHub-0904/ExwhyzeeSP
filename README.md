# ExwhyzeeSP (Enterprise School Portal & Management Platform)

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13-239120.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Entity Framework Core 9](https://img.shields.io/badge/EF%20Core-9.0-purple.svg)](https://docs.microsoft.com/en-us/ef/core/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20Modular%20Monolith-orange.svg)]()
[![Platform](https://img.shields.io/badge/Platform-Cross--Platform%20(Linux%20%2F%20Windows)-lightgrey.svg)]()
[![Production Proven](https://img.shields.io/badge/Production%20Proven-110%2B%20Institutions%20%7C%207%2B%20Years-success.svg)]()

---

## 📌 Executive Summary

**ExwhyzeeSP** is an enterprise-grade, multi-tenant School Management System and Content Management Platform designed to automate educational operations end-to-end. Engineered for high scalability, fault tolerance, and institutional reliability, the platform powers administrative, academic, financial, and digital presence workflows for primary, secondary, and tertiary educational institutions.

### 🌟 Proven Track Record
- **7+ Years of Continuous Production Operation:** Established and actively evolved since 2019 to handle high-stakes academic cycles, nationwide result checking, and high-concurrency admission drives.
- **110+ Educational Institutions Serviced:** Field-tested across over 110 schools and learning academies, processing hundreds of thousands of student profiles, assessments, report cards, and digital transcripts.
- **White-Label Architecture:** Fully white-labeled; each institution runs under its custom domain, visual identity, branding assets, ribbon notifications, and layout templates with zero disclosure of underlying cloud infrastructure.

---

## 🔄 Architectural Evolution: From .NET Framework 4.6 to .NET 9.0

Originally engineered on the legacy **.NET Framework 4.6 (ASP.NET MVC 5 & EF6)**, the platform underwent a multi-phase modernization to **.NET 9.0**, achieving dramatic performance leaps, cross-platform hostability, and enterprise modularity.

```
┌────────────────────────────────────────────────────────┐
│                   LEGACY ARCHITECTURE                  │
│  .NET Framework 4.6  •  ASP.NET MVC 5  •  EF 6         │
│  Windows / IIS Dependent  •  Synchronous File I/O      │
└───────────────────────────┬────────────────────────────┘
                            │
              MAJOR RE-ENGINEERING & UPGRADE
                            │
                            ▼
┌────────────────────────────────────────────────────────┐
│                    MODERN .NET 9.0                     │
│  ASP.NET Core 9  •  EF Core 9  •  C# 13                │
│  Cross-Platform (Linux Ubuntu + Windows Kestrel)      │
│  High-Performance Asynchronous I/O Pipeline           │
│  Direct S3 Cloud Storage & Multi-Tenant Routing        │
└────────────────────────────────────────────────────────┘
```

### Key Migration Milestones & Engineering Upgrades:
1. **Framework Modernization:** Upgraded all solution components from `.NET Framework 4.6.1` to `.NET 9.0`, leveraging C# 13 features (nullable reference types, pattern matching, record types, and file-scoped namespaces).
2. **Data Access Modernization:** Migrated from legacy Entity Framework 6 (`DbContext` / EDMX / ObjectContext) to **Entity Framework Core 9.0**, implementing compiled queries, batch execution, connection pooling, and resilient execution strategies.
3. **Cross-Platform Portability:** Completely decoupled the codebase from Windows-specific APIs (`System.Web`, Registry, Windows Event Log, and IIS-specific HTTP Modules). Fully containerizable and production-ready on Linux (Ubuntu Kestrel + Systemd + Nginx).
4. **Identity & Security Re-architecture:** Transitioned from ASP.NET Identity 2.x to modern **ASP.NET Core Identity**, preserving full hash compatibility across legacy institution databases while adding Claims-based authorization and granular RBAC.
5. **Decoupled Cloud Storage:** Eliminated local file storage dependencies (`wwwroot/uploads`) in favor of direct **Amazon S3 Cloud Object Storage** with multi-year secure pre-signed URLs, preventing disk saturation and enabling stateless horizontal scaling.
6. **Zero-Configuration Environment Management:** Replaced rigid `web.config` appSettings with **Twelve-Factor App `.env` and `IConfiguration`** loading patterns.

---

## 🏗 Solution Structure & System Architecture

The solution adheres to Clean Architecture and Modular Monolith principles, separating concerns across distinct layers:

```
src/
├── ExwhyzeeSP.Core/          # Domain Entities, Models, Enums, DTOs & Business Contracts
├── ExwhyzeeSP.Data/          # EF Core DbContext, Migrations, Multi-Tenant Context Resolvers
├── ExwhyzeeSP.Services/      # Business Logic Services, AWS S3 Storage, Communication APIs
├── ExwhyzeeSP.Master/        # Multi-Tenant Master Orchestrator & Auto-Provisioning Engine
└── ExwhyzeeSP.Web/           # Presentation Layer (Razor Pages, MVC Areas, REST Endpoints)
    ├── Areas/
    │   ├── Admin/            # Institutional Administration (Academics, Staff, Finance)
    │   ├── MisContent/       # CMS Engine (Website Settings, Theme Selector, Slider Manager)
    │   ├── School/           # Student & Class Management Workflows
    │   ├── Result/           # Grading, Continuous Assessment, Report Card Generation
    │   └── Service/          # Background Services, Health Checks, Diagnostics
    ├── Pages/                # Public Institutional Website & Dynamic CMS Views
    └── wwwroot/              # Static Frontend Assets (AdminLTE, RedBiz Theme, Plugins)
```

### Architectural Breakdown:

| Project | Responsibility | Key Technologies |
| :--- | :--- | :--- |
| **`ExwhyzeeSP.Core`** | Pure Domain Layer. Contains entity models (Student, Staff, Course, Grade, Slider, Setting, etc.), enums, and request/response DTOs. | .NET 9.0, C# 13 |
| **`ExwhyzeeSP.Data`** | Persistence Layer. Manages `ApplicationDbContext`, entity relationships, migrations, and tenant connection string resolution. | EF Core 9.0, SQL Server |
| **`ExwhyzeeSP.Services`** | Application Services. S3 Storage Provider, Email (SMTP/Webmail), WhatsApp Cloud API, SMS Gateways, Payment Verification. | AWS SDK, REST Clients |
| **`ExwhyzeeSP.Master`** | Tenant Management. Handles institution domain mapping, database auto-provisioning, tenant backups, and system-wide licensing. | ASP.NET Core, EF Core |
| **`ExwhyzeeSP.Web`** | Web Application. Combines Razor Pages for CMS and public portals with MVC Areas for secure Administrative Dashboards. | Razor Pages, MVC, Kestrel |

---

## ⚡ Core Functional Modules

### 1. 🎓 Student Information & Enrollment (SIS)
- Comprehensive bio-data, parental links, medical records, and document attachments.
- Class and arm allocation, bulk student promotions, graduation workflows, and alumni tracking.
- Unique student identification numbers and dynamic profile generation.

### 2. 📊 Academic Continuous Assessment & Result Engine
- Flexible grading systems adaptable to custom scoring schemes (e.g., 20/20/60 or 30/70).
- Automated cumulative assessment computation, position/rank determination, and grade remarks.
- High-fidelity **Report Card PDF generation** with institution branding, digital signatures, and attendance summaries.
- **Scratch Card PIN Verification System:** Secure result checker portal gated by one-time or multi-use PIN cards to prevent unauthorized grade alterations.

### 3. 🌐 Dynamic Institutional CMS & Theme Engine
- Integrated public website builder with customizable header navigations, dynamic sub-pages, categories, and ribbon notifications.
- **Multi-Layout Support:** Switchable visual themes (e.g., RedBiz, Modern Education, Classic Portal).
- **Hero Slider Engine:** Dynamic carousel supporting custom text, overlay effects, video buttons, and direct action links.
- Institutional identity controls: Primary logo, White logo (for dark headers), Favicons, and UI properties.

### 4. 👥 Staff, HR & Payroll Management
- Employee profile tracking, role assignments (Principal, Teacher, Accountant, Editor, Manager).
- Subject and class teacher assignments with granular permission boundaries.
- Payroll schedules, allowances, deductions, and payslip distribution.

### 5. 💳 Financials, Tuition & Fee Collection
- Term-by-term fee scheduling with customizable billing items per class/arm.
- Online payment gateway integration (Paystack, Flutterwave, Monnify, Bank Transfer tracking).
- Automated receipting, outstanding balance reconciliation, and financial audit logs.

### 6. 📱 Omnichannel Communication Suite
- **Bulk SMS Integration:** Instant notification dispatch for emergencies, term notices, and fee reminders.
- **Transactional Email Gateway:** Supports SMTP, Webmail, Google, and Outlook servers with SSL/TLS.
- **WhatsApp Cloud API Integration:** Automated result dispatch and admission updates directly to parent WhatsApp numbers.

### 7. ☁️ Enterprise Cloud Storage (S3 Engine)
- Zero-disk reliance: all logos, sliders, assignment attachments, and student credentials upload directly to cloud object storage.
- Auto-generation of secure, long-lived pre-signed URLs with automatic expiration handling.
- Integrated file size controls, MIME-type validation, and request limits.

---

## 🚀 Getting Started (Development Setup)

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (Version 9.0.100 or later)
- [Microsoft SQL Server 2019+](https://www.microsoft.com/sql-server/) (or Azure SQL / AWS RDS)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.12+) or [VS Code](https://code.visualstudio.com/) with C# Dev Kit
- [Git](https://git-scm.com/)

### 1. Clone the Repository
```bash
git clone https://github.com/SirP-TechHub-0904/ExwhyzeeSP.git
cd ExwhyzeeSP
```

### 2. Configure Environment Variables
Create a `.env` file in the solution root or inside `src/ExwhyzeeSP.Web/` (a template is provided in `.env.example`):

```ini
# ==============================================================================
# Database Connections
# ==============================================================================
DEFAULT_DB_CONNECTION="Server=localhost;Database=ExwhyzeeSP_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
MASTER_DB_CONNECTION="Server=localhost;Database=ExwhyzeeSP_Master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"

# ==============================================================================
# Security & Master Platform
# ==============================================================================
PLATFORM_NAME="School Portal Management Platform"
PLATFORM_ROOT_DOMAIN="example.com"
APP_SECRET_KEY="YourSecurePlatformKey_ChangeInProduction!"
MASTER_ADMIN_EMAIL="admin@example.com"
MASTER_ADMIN_DEFAULT_PASSWORD="YourSecurePassword123!"

# ==============================================================================
# Cloud Storage Configuration (S3)
# ==============================================================================
AwsConfiguration__AWSAccessKey="YOUR_AWS_ACCESS_KEY"
AwsConfiguration__AWSSecretKey="YOUR_AWS_SECRET_KEY"
AwsConfiguration__BucketName="your-cloud-bucket"

# ==============================================================================
# Communication Integrations (SMS, Payment)
# ==============================================================================
SMS_API_KEY="your_sms_gateway_key"
SMS_SENDER_ID="SCHOOLNAME"
PAYSTACK_SECRET_KEY="sk_live_xxxx"
PAYSTACK_PUBLIC_KEY="pk_live_xxxx"
```

> [!IMPORTANT]
> The `.env` file is excluded from Git via `.gitignore`. Never commit raw API keys or passwords to version control.

### 3. Build & Apply Migrations
```bash
# Restore dependencies and build solution
dotnet restore
dotnet build

# Apply database migrations
cd src/ExwhyzeeSP.Web
dotnet ef database update --project ../ExwhyzeeSP.Data
```

### 4. Run Locally
```bash
dotnet run --project src/ExwhyzeeSP.Web
```
Access the application locally at `https://localhost:7230` or `http://localhost:5013`.

---

## 🌐 Production Deployment (Linux / Ubuntu VPS)

The platform is optimized for deployment on Linux servers using Kestrel behind an Nginx reverse proxy managed by Systemd.

### 1. Publish Build
```bash
dotnet publish src/ExwhyzeeSP.Web/ExwhyzeeSP.Web.csproj -c Release -o /var/www/schoolportal-web
```

### 2. Systemd Service Definition (`/etc/systemd/system/schoolportal.service`)
```ini
[Unit]
Description=ExwhyzeeSP Enterprise School Portal
After=network.target

[Service]
WorkingDirectory=/var/www/schoolportal-web
ExecStart=/usr/bin/dotnet /var/www/schoolportal-web/ExwhyzeeSP.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=schoolportal
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### 3. Nginx Reverse Proxy Configuration
```nginx
server {
    server_name portal.institution.edu.ng www.portal.institution.edu.ng;
    client_max_body_size 100M;

    location / {
        proxy_pass         http://127.0.0.1:5013;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Real-IP $remote_addr;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_set_header   X-Forwarded-Host $host;
    }

    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/portal.institution.edu.ng/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/portal.institution.edu.ng/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;
}
```

---

## 🔒 Security & Data Hygiene

- **Strict Cloud Storage Policy:** Zero local fallback for uploaded assets. All media files are streamed directly to authenticated cloud object stores.
- **Client Confidentiality:** White-label architecture preserves institutional independence; client identities, databases, and portal credentials remain strictly segregated.
- **Credential Hygiene:** No hardcoded secrets in source files or configuration JSONs. Secrets are managed dynamically via runtime environment variables.
- **Data Protection:** Multi-tenant database isolation ensures student academic records and financial ledgers remain fully segregated across instances.

---

## 📄 License & Intellectual Property

Copyright © 2019–2026 **Exwhyzee Technologies / Platform Team**. All rights reserved.  
Confidential and Proprietary. Unauthorized copying, distribution, modification, or deployment of this software via any medium is strictly prohibited without explicit written consent.
