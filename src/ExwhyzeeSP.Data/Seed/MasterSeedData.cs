using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Data.Seed
{
    /// <summary>
    /// Seeds all schools, connection strings, Identity RBAC roles, default SuperAdmin, and financial ledger data.
    /// </summary>
    public static class MasterSeedData
    {
        public static async Task InitializeAsync(MasterDbContext context, IServiceProvider? serviceProvider = null)
        {
            // Ensure Master database tables are created locally
            await context.Database.EnsureCreatedAsync();

            // Ensure FinTech soft-delete columns exist on TenantSchools
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF COL_LENGTH('dbo.TenantSchools', 'IsDeleted') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [IsDeleted] bit NOT NULL CONSTRAINT DF_TenantSchools_IsDeleted DEFAULT(0);
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletedAt') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletedAt] datetime2 NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletedBy') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletedBy] nvarchar(200) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletionReason') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletionReason] nvarchar(500) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupDate') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupDate] datetime2 NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupUrl') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupUrl] nvarchar(500) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupSize') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupSize] nvarchar(50) NULL;
                    END
                ");
            }
            catch
            {
                // Suppressed if table not yet created or permissions restricted
            }

            // Seed RBAC Identity Roles and SuperAdmin
            if (serviceProvider != null)
            {
                await SeedIdentityAndFinancialsAsync(context, serviceProvider);
            }

            // Seed Testimonies if empty
            if (!await context.Testimonies.AnyAsync())
            {
                var defaultTestimonies = new List<Testimony>
                {
                    new Testimony
                    {
                        Title = "Dr. Anthony Uche",
                        Position = "Director of Academics",
                        SchoolName = "Premier International Academy",
                        Rating = 5,
                        Show = true,
                        SortOrder = "1",
                        Description = "Result compilation and computational errors used to give us sleepless nights. Exwhyzee SP automated our terminal report cards, affective domains, and class positions with pinpoint accuracy.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Testimony
                    {
                        Title = "Mrs. Folashade Adeleke",
                        Position = "Head of Admissions",
                        SchoolName = "Grace Model College",
                        Rating = 5,
                        Show = true,
                        SortOrder = "2",
                        Description = "The digital admission and entrance examination module saved our administrative team hundreds of hours. Parents applied online, paid acceptance fees, and printed offer letters immediately.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Testimony
                    {
                        Title = "Alhaji Bello Mohammed",
                        Position = "School Proprietor",
                        SchoolName = "Al-Hikmah Comprehensive College",
                        Rating = 5,
                        Show = true,
                        SortOrder = "3",
                        Description = "Fee defaulters tracking and automated SMS payment reminders improved our termly fee collection rate to 96%. The financial reporting tools give me complete peace of mind.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Testimony
                    {
                        Title = "Engr. Emeka Okonkwo",
                        Position = "ICT Coordinator",
                        SchoolName = "St. Paul's High School",
                        Rating = 5,
                        Show = true,
                        SortOrder = "4",
                        Description = "Our students love the online CBT exam interface. It prepares them seamlessly for external computer-based exams (JAMB, NECO, WAEC) with real-time scoring and instant feedback.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Testimony
                    {
                        Title = "Dr. (Mrs) Chioma Igwe",
                        Position = "Principal",
                        SchoolName = "Holy Child Secondary School",
                        Rating = 5,
                        Show = true,
                        SortOrder = "5",
                        Description = "Parents appreciate receiving automated terminal broadsheets and emergency updates directly on their phones via SMS with our school's branded Sender ID.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Testimony
                    {
                        Title = "Pastor Timothy Johnson",
                        Position = "Boarding House Master",
                        SchoolName = "Bethany Christian Academy",
                        Rating = 5,
                        Show = true,
                        SortOrder = "6",
                        Description = "Managing multiple boarding houses and hostel bed allotments used to be chaotic. With Exwhyzee SP, room occupancy and boarding clearances are tracked automatically.",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Testimonies.AddRangeAsync(defaultTestimonies);
                await context.SaveChangesAsync();
            }

            // Seed Master CMS Pages (Privacy Policy, Terms of Service, Cookie Policy) if empty
            if (!await context.MasterCmsPages.AnyAsync())
            {
                var defaultPages = new List<MasterCmsPage>
                {
                    new MasterCmsPage
                    {
                        Slug = "privacy-policy",
                        Title = "Privacy Policy",
                        Summary = "Comprehensive data protection, student record privacy, multi-tenant database isolation, and NDPR/GDPR compliance guidelines for Exwhyzee SP.",
                        Content = @"
<h3 id=""introduction"">1. Introduction &amp; Commitment</h3>
<p>Exwhyzee Technologies (&quot;Exwhyzee SP&quot;, &quot;we&quot;, &quot;our&quot;, or &quot;us&quot;) operates a unified, multi-tenant cloud School Management and Academic Enterprise Resource Planning (ERP) platform. We are committed to protecting the privacy, integrity, and confidentiality of student records, educational assessment data, parent communications, and institutional financial transactions.</p>
<p>This Privacy Policy explains how personal data is collected, stored, encrypted, processed, and safeguarded when schools, administrators, teachers, parents, guardians, and students access and utilize our services.</p>

<h3 id=""information-we-collect"">2. Categories of Information We Collect</h3>
<p>To provide school management services, we process several categories of data entrusted to us by educational institutions:</p>
<ul>
    <li><strong>Student Profile &amp; Academic Data:</strong> Full legal names, admission/matriculation numbers, gender, date of birth, passport photographs, continuous assessment (CA) scores, examination marks, grade positions, psychomotor ratings, and terminal broadsheets.</li>
    <li><strong>Parent &amp; Guardian Information:</strong> Full names, primary phone numbers (for SMS broadcasts and notifications), email addresses, residential locations, and fee payment verification transaction records.</li>
    <li><strong>Staff &amp; Educator Credentials:</strong> Official email addresses, telephone numbers, role-based access rights, lesson plan submissions, attendance records, and administrative audit trails.</li>
    <li><strong>System &amp; Usage Telemetry:</strong> Log timestamps, IP addresses, browser types, session duration, and device identifiers collected to prevent brute-force attacks and ensure account safety.</li>
</ul>

<h3 id=""how-we-use-data"">3. Purpose and Legal Basis for Processing</h3>
<p>All data processed on Exwhyzee SP is governed by contractual necessity with registered educational institutions and used solely for lawful academic and administrative operations, including:</p>
<ul>
    <li>Processing terminal report cards, cumulative transcripts, and automated cognitive evaluations.</li>
    <li>Conducting Computer-Based Testing (CBT) examinations, online quizzes, and entrance assessment grading.</li>
    <li>Invoicing school fees, issuing digital receipts, generating bank tellers, and reconciling payments.</li>
    <li>Delivering automated SMS alerts, exam timetable broadcasts, fee reminder alerts, and emergency notifications.</li>
    <li>Facilitating prospective student admissions, entrance screening forms, and registration verification.</li>
</ul>

<h3 id=""data-isolation-security"">4. Multi-Tenant Data Isolation &amp; Security Architecture</h3>
<p>Security is embedded at every layer of our platform architecture:</p>
<ul>
    <li><strong>Database Isolation:</strong> Each institution operates with dedicated database connection strings or isolated schemas, preventing cross-tenant data leakage or unauthorized access.</li>
    <li><strong>Encryption in Transit &amp; at Rest:</strong> All web traffic is encrypted using 256-bit TLS/SSL encryption. Database backups and sensitive credentials are encrypted using industry-standard hashing algorithms.</li>
    <li><strong>Role-Based Access Control (RBAC):</strong> Strict permission matrices ensure teachers only access their assigned classes, students only view their own performance, and accountants only access financial ledgers.</li>
    <li><strong>Automated Disaster Recovery:</strong> Redundant cloud snapshots and point-in-time recovery mechanisms ensure business continuity and guard against catastrophic data loss.</li>
</ul>

<h3 id=""financial-data"">5. Financial &amp; Payment Data Security</h3>
<p>Exwhyzee SP does not store raw credit/debit card numbers or bank account PINs on our servers. All online fee transactions are processed through certified, PCI-DSS compliant payment gateways. We only retain transaction reference IDs, settlement timestamps, and bank payment confirmation logs for accounting and audit reconciliation.</p>

<h3 id=""data-retention"">6. Data Retention &amp; Institutional Ownership</h3>
<p>Educational institutions retain complete, exclusive ownership of their student and staff data. Exwhyzee SP acts as a Data Processor. We retain academic archives for the duration of the school's active subscription and in accordance with institutional regulatory standards. School administrators may request full database exports or account archiving at the conclusion of any academic session.</p>

<h3 id=""rights-of-users"">7. Data Subject Rights &amp; Regulatory Compliance</h3>
<p>We comply with the Nigeria Data Protection Regulation (NDPR) and international privacy frameworks (GDPR). Data subjects (students, parents, and teachers) have the right to request access, rectification, or updates to their personal information through their school administrator or our Data Protection Officer.</p>

<h3 id=""cookies-analytics"">8. Cookies and Storage Technologies</h3>
<p>We use essential cookies and browser storage tokens to authenticate users, protect forms from Cross-Site Request Forgery (CSRF), and maintain session state. Please review our <a href=""/Cookies"">Cookie Policy</a> for complete details.</p>

<h3 id=""contact-privacy"">9. Contact Our Data Protection Officer</h3>
<p>If you have questions, feedback, or compliance requests regarding this Privacy Policy, please contact our Data Protection Team:</p>
<p><strong>Exwhyzee Technologies Ltd</strong><br />
Email: <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a> | <a href=""mailto:privacy@exwhyzee.ng"">privacy@exwhyzee.ng</a><br />
Website: <a href=""https://exwhyzee.ng"" target=""_blank"">www.exwhyzee.ng</a><br />
Support Line: +234 800 EXWHYZEE (24/7 Academic Support)</p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    },
                    new MasterCmsPage
                    {
                        Slug = "terms-and-conditions",
                        Title = "Terms of Service",
                        Summary = "Rules of engagement, institutional licensing, subscription terms, SLA commitments, and acceptable use policy for Exwhyzee SP.",
                        Content = @"
<h3 id=""agreement-to-terms"">1. Acceptance of Terms</h3>
<p>These Terms of Service (&quot;Agreement&quot;) constitute a legally binding contract between Exwhyzee Technologies Ltd (&quot;Exwhyzee&quot;, &quot;we&quot;, &quot;us&quot;) and the subscribing educational institution, school administrator, educator, parent, or student (&quot;Customer&quot;, &quot;User&quot;, &quot;You&quot;) accessing the Exwhyzee School Portal (Exwhyzee SP) software platform.</p>
<p>By registering a school tenant, logging into the portal, or purchasing a subscription, you confirm that you have read, understood, and agreed to be bound by these Terms.</p>

<h3 id=""provisioning-tenancy"">2. Tenant Provisioning &amp; Authorized Access</h3>
<p>Upon registration and verification, Exwhyzee provisions a dedicated sub-domain (e.g., <code>yourschool.exwhyzee.ng</code>) or custom domain mapping linked to a secure database instance. The subscribing institution is responsible for maintaining the confidentiality of administrative master passwords, staff PINs, and student access scratch cards.</p>

<h3 id=""sla-uptime"">3. Service Level Agreement (SLA) &amp; System Availability</h3>
<p>We commit to a <strong>99.9% application uptime</strong> throughout the active academic session, excluding scheduled maintenance windows. Scheduled maintenance is typically conducted during off-peak weekend hours with advance email and in-app dashboard notification.</p>

<h3 id=""billing-subscriptions"">4. Pricing, Subscriptions &amp; Billing Terms</h3>
<p>Exwhyzee SP offers flexible educational billing models designed to accommodate private and public institutions of all sizes:</p>
<ul>
    <li><strong>Per-Student Per-Term Model:</strong> Invoiced based on the number of active enrolled students at each term's census date.</li>
    <li><strong>Per-Classroom Flat Model:</strong> Predictable tier-based pricing per active academic classroom or branch.</li>
    <li><strong>Annual Institutional License:</strong> Discounted flat annual subscription covering unlimited students, CBT, and admissions.</li>
    <li><strong>Digital Result Scratch Cards:</strong> Pay-as-you-go PIN verification cards for end-of-term student result access and online verification.</li>
</ul>
<p>All subscription payments are due within the invoice grace period. Services may be placed in read-only mode if invoices remain delinquent past the agreed term deadline.</p>

<h3 id=""acceptable-use"">5. Acceptable Use Policy &amp; Platform Integrity</h3>
<p>Users agree not to:</p>
<ul>
    <li>Attempt unauthorized penetration testing, vulnerability scanning, or SQL injection attacks against the infrastructure.</li>
    <li>Upload malicious scripts, viruses, or unauthorized exam materials.</li>
    <li>Use portal SMS broadcast channels to send unsolicited spam, political advertising, or fraudulent notices.</li>
    <li>Share administrative master credentials outside of authorized institutional staff.</li>
</ul>

<h3 id=""intellectual-property"">6. Proprietary Software Rights</h3>
<p>All source code, software architecture, CBT algorithms, custom report card template engines, user interfaces, branding, and documentation are the exclusive intellectual property of Exwhyzee Technologies Ltd. Subscribing institutions are granted a non-exclusive, non-transferable license to access and use the software during the active subscription period.</p>

<h3 id=""data-ownership-backup"">7. Academic Data Ownership &amp; Backups</h3>
<p>The subscribing school maintains full legal ownership of all student bio-data, academic grades, examination questions, and financial records uploaded to their instance. Exwhyzee performs automated daily snapshots to cloud vaults. In the event of contract termination, schools may request complete data exports in SQL/CSV/Excel format.</p>

<h3 id=""limitation-liability"">8. Limitation of Liability</h3>
<p>To the maximum extent permitted by applicable law, Exwhyzee Technologies shall not be liable for indirect, punitive, or consequential damages resulting from internet service provider downtime, unauthorized user credential sharing, or delayed third-party SMS gateway delivery. Our aggregate liability is strictly limited to the subscription fees paid by the institution during the preceding term.</p>

<h3 id=""dispute-resolution"">9. Governing Law &amp; Dispute Resolution</h3>
<p>This Agreement is governed by the laws of the Federal Republic of Nigeria. Any disputes arising from or in connection with these Terms shall first be resolved through good-faith mutual consultation, and if unresolved, referred to binding arbitration in accordance with the Arbitration and Conciliation Act.</p>

<h3 id=""contact-legal"">10. Contact Information</h3>
<p>For questions regarding these Terms of Service or enterprise licensing agreements, please contact:</p>
<p><strong>Legal &amp; Contracts Department</strong><br />
Exwhyzee Technologies Ltd<br />
Email: <a href=""mailto:legal@exwhyzee.ng"">legal@exwhyzee.ng</a> | <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a><br />
Website: <a href=""https://exwhyzee.ng"" target=""_blank"">www.exwhyzee.ng</a></p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    },
                    new MasterCmsPage
                    {
                        Slug = "cookie-policy",
                        Title = "Cookie Policy",
                        Summary = "How Exwhyzee SP uses cookies, local storage tokens, and browser cache to ensure security, high performance, and reliable session authentication.",
                        Content = @"
<h3 id=""what-are-cookies"">1. What Are Cookies and Local Storage?</h3>
<p>Cookies and browser storage mechanisms (such as <code>localStorage</code> and session tokens) are small data files stored on your computer or mobile device when you browse websites and use cloud applications. They allow our platform to remember your login session, prevent repeated logins, and safeguard your account against unauthorized requests.</p>

<h3 id=""cookies-we-use"">2. Categories of Cookies We Utilize</h3>
<p>Exwhyzee SP uses strictly necessary and functional cookies for portal operations:</p>
<ul>
    <li><strong>Essential Authentication Cookies:</strong> Cryptographically signed session identifiers that authenticate teachers, school administrators, students, and parents across pages without requiring re-login on every click.</li>
    <li><strong>Security &amp; Anti-Forgery Tokens (CSRF):</strong> Cryptographic tokens that verify form submissions (such as student score entry, CBT quiz submissions, and fee payments) originate genuinely from your browser, protecting against malicious cross-site attacks.</li>
    <li><strong>User Preference Tokens:</strong> Store interface preferences, such as selected academic sessions, active class filters, table pagination sizes, and your cookie consent status.</li>
    <li><strong>Performance &amp; Cache State:</strong> Ephemeral browser tokens used to cache report card assets, minimizing load times over mobile broadband networks.</li>
</ul>

<h3 id=""third-party-cookies"">3. Third-Party Cookies &amp; Integrations</h3>
<p>When processing tuition fees online, integrated payment gateways (such as Flutterwave, Paystack, or Interswitch) may set temporary security cookies to detect fraudulent transactions and verify two-factor authentication (2FA). Exwhyzee SP does not use intrusive third-party cross-site advertising tracking cookies.</p>

<h3 id=""managing-cookies"">4. How You Can Manage Cookies</h3>
<p>You can configure or adjust cookie settings directly within your web browser settings. Most modern browsers (Chrome, Edge, Firefox, Safari) allow you to block or delete cookies. However, please note that disabling essential authentication cookies will prevent you from logging into the portal, submitting CBT exams, or viewing academic report cards.</p>

<h3 id=""policy-updates"">5. Updates to This Cookie Policy</h3>
<p>We may update this policy periodically to reflect enhancements in technology, security standards, or regulatory guidelines. Updates take effect immediately upon publication on this page.</p>

<h3 id=""contact-cookies"">6. Questions &amp; Support</h3>
<p>For questions regarding our use of cookies and privacy safeguards, please reach out to our technical team at <a href=""mailto:support@exwhyzee.ng"">support@exwhyzee.ng</a> or <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a>.</p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    }
                };

                await context.MasterCmsPages.AddRangeAsync(defaultPages);
                await context.SaveChangesAsync();
            }

            // Seed Blog Categories and Educational Articles if empty
            if (!await context.BlogCategories.AnyAsync())
            {
                var catManagement = new BlogCategory { Title = "School Management", Publish = true };
                var catCbt = new BlogCategory { Title = "CBT & Examinations", Publish = true };
                var catTech = new BlogCategory { Title = "EdTech Innovation", Publish = true };
                var catFinance = new BlogCategory { Title = "Fee Billing & Finance", Publish = true };

                await context.BlogCategories.AddRangeAsync(catManagement, catCbt, catTech, catFinance);
                await context.SaveChangesAsync();

                var defaultBlogs = new List<Blog>
                {
                    new Blog
                    {
                        Title = "How Automated Report Cards Eliminate End-of-Term Computation Stress",
                        Date = DateTime.UtcNow.AddDays(-12),
                        Publish = true,
                        SortOrder = 1,
                        BlogCategoryId = catManagement.Id,
                        ImageUrl = "/assets/images/blog-image1.jpg",
                        Description = "Discover how cloud continuous assessment scoring and automated grading curves save teachers over 40 hours every examination cycle.",
                        FullDescription = @"
<p>End-of-term computation in primary and secondary schools has historically been one of the most grueling exercises for educators. Manual collation of continuous assessments, calculating cumulative averages across terms, compiling class broadsheets, and writing out physical report cards by hand is fraught with computational errors and exhausting fatigue.</p>
<h3>1. 100% Error-Free Automatic Computation</h3>
<p>Exwhyzee SP replaces manual arithmetic with instant algorithmic computation. As soon as subject teachers enter test scores and exam marks, the engine calculates positions, grade remarks, class averages, and GPA metrics based on your school's customized grading scale.</p>
<h3>2. Instant Cumulative Broadsheets for Principals</h3>
<p>School administrators can instantly review, moderate, and approve class broadsheets with a single click. Any flagged discrepancies or missing scores are immediately highlighted before report cards are finalized.</p>
<h3>3. Digital Verification via Scratch Cards &amp; QR Codes</h3>
<p>Each generated report card comes with a unique verification QR code and secure digital scratch-card PIN verification, preventing report forgery and giving parents instant access from their smartphones anywhere in the world.</p>
",
                        AddToRibon = true,
                        RibonCustomDisplayTitle = "Featured Case Study",
                        RibonSortOrder = 1
                    },
                    new Blog
                    {
                        Title = "Securing School Fee Collections with Automated Multi-Channel Payment Reconciliation",
                        Date = DateTime.UtcNow.AddDays(-8),
                        Publish = true,
                        SortOrder = 2,
                        BlogCategoryId = catFinance.Id,
                        ImageUrl = "/assets/images/blog-image2.jpg",
                        Description = "A comprehensive guide for school proprietors on stopping revenue leakages, automating fee invoices, and tracking termly payment defaults.",
                        FullDescription = @"
<p>Managing school tuition fee collection across hundreds or thousands of students is a delicate financial task. Traditional paper bank tellers are frequently vulnerable to forgery, delayed bank reconciliation, and accounting discrepancies that strain school operating budgets.</p>
<h3>1. Automated Invoicing and Fee Schedules</h3>
<p>With Exwhyzee SP, fee schedules can be tailored per class, hostel status (day vs. boarding), and optional services like school bus transportation, uniform fees, and excursion costs. Invoices are automatically tied to parent accounts with clear line-item breakdowns.</p>
<h3>2. Real-Time Bank Settlement &amp; Automated Receipts</h3>
<p>Whether parents pay online with cards, USSD, or direct bank transfer, payments are verified in real time. Digital receipts are generated instantly and dispatched via SMS and email, updating the bursar's dashboard automatically.</p>
<h3>3. Transparent Debt Tracking &amp; Defaulter Notifications</h3>
<p>The bursary module allows administrators to view real-time debtor lists, send automated reminder SMS broadcasts, and conditionally restrict exam cards or termly result access for unpaid accounts.</p>
",
                        AddToRibon = true,
                        RibonCustomDisplayTitle = "Financial Management",
                        RibonSortOrder = 2
                    },
                    new Blog
                    {
                        Title = "The Power of Integrated CBT: Preparing Students for JAMB, WAEC, and Global Exams",
                        Date = DateTime.UtcNow.AddDays(-4),
                        Publish = true,
                        SortOrder = 3,
                        BlogCategoryId = catCbt.Id,
                        ImageUrl = "/assets/images/blog-image3.jpg",
                        Description = "Why modern schools are migrating classroom tests and mid-term assessments into timed Computer-Based Testing systems.",
                        FullDescription = @"
<p>In modern education, standardized examinations like JAMB UTME, WAEC, and international certifications are completely computer-based. Students who only experience pencil-and-paper exams during their secondary education often struggle with timed computer interfaces, question navigation, and digital test anxiety.</p>
<h3>1. Simulating Real-World Examination Interfaces</h3>
<p>Exwhyzee SP's CBT engine provides timed exam sessions, randomized question orders, shuffled answer options, and instant automated grading. Students gain immense digital confidence by taking their weekly quizzes and mid-term tests directly on computers and tablets.</p>
<h3>2. Rich Multimedia Question Banks</h3>
<p>Educators can build comprehensive question repositories containing mathematical equations, diagrams, audio comprehension clips, and multiple-choice or short-answer options.</p>
<h3>3. Instant Result Analytics &amp; Subject Weakness Reports</h3>
<p>Upon exam submission, teachers receive instant analytics showing question difficulty distribution, subject pass percentages, and specific student weak points, enabling targeted academic intervention before external examinations.</p>
",
                        AddToRibon = true,
                        RibonCustomDisplayTitle = "CBT & Assessment",
                        RibonSortOrder = 3
                    },
                    new Blog
                    {
                        Title = "10 Transformative EdTech Trends Shaping Nigerian Schools in 2026",
                        Date = DateTime.UtcNow.AddDays(-1),
                        Publish = true,
                        SortOrder = 4,
                        BlogCategoryId = catTech.Id,
                        ImageUrl = "/assets/images/blog-image4.jpg",
                        Description = "From multi-tenant cloud portals and automated SMS gateways to smart biometric attendance and paperless admissions.",
                        FullDescription = @"
<p>The Nigerian educational landscape is undergoing an unprecedented digital revolution. Leading private and mission schools are rapidly adopting unified cloud architectures that connect proprietors, teachers, parents, and students in real time.</p>
<h3>1. Unified Multi-Tenant Cloud Portals</h3>
<p>Schools no longer need expensive on-premise servers or fragile local hard drives. Cloud platforms ensure all academic data is secured in world-class datacenters with 99.9% uptime and automatic daily backups.</p>
<h3>2. Paperless Digital Admissions &amp; Online Entrance Screening</h3>
<p>Parents can complete entrance application forms, upload credentials, pay application fees, and schedule entrance CBT interviews without traveling across states or queuing in school administrative offices.</p>
<h3>3. 24/7 Parent Engagement via Targeted SMS &amp; Portal Portals</h3>
<p>Transparent communication fosters parent trust. Real-time updates on daily attendance, exam dates, disciplinary remarks, and academic progress strengthen parent-school partnerships and enhance student outcomes.</p>
",
                        AddToRibon = false,
                        RibonSortOrder = 4
                    }
                };

                await context.Blogs.AddRangeAsync(defaultBlogs);
                await context.SaveChangesAsync();
            }

            // Seed Master Pricing Plans if empty
            if (!await context.PricingPlans.AnyAsync())
            {
                var defaultPlans = new List<MasterPricingPlan>
                {
                    new MasterPricingPlan
                    {
                        Name = "Pay Per Student",
                        Subtitle = "Ideal for nursery, primary & secondary schools paying strictly per enrolled child.",
                        PriceText = "₦500 - ₦1,500",
                        BillingCycle = "/ student / term",
                        Description = "Pay as you grow with flexible census-based billing calculated at the start of each term.",
                        BadgeText = "Termly Flexibility",
                        IsPopular = false,
                        ButtonText = "Get Started",
                        ButtonUrl = "/Demo",
                        Features = "Student & Staff Information System\nAutomated Terminal Report Cards & Broadsheets\nOnline Admissions & Application Screening\nFee Billing & Payment Tracking\nParent SMS & Email Broadcast Alerts\nOnline CBT Quizzes & Assessments\nAI Learning & Holiday Skills Hub",
                        SortOrder = 1,
                        IsActive = true
                    },
                    new MasterPricingPlan
                    {
                        Name = "Pay Per Classroom",
                        Subtitle = "Flat classroom tier for schools wanting fixed, predictable termly expenses.",
                        PriceText = "₦15,000 - ₦30,000",
                        BillingCycle = "/ classroom / term",
                        Description = "Fixed cost per active classroom arm regardless of student population growth during the session.",
                        BadgeText = "Most Popular",
                        IsPopular = true,
                        ButtonText = "Choose Classroom Plan",
                        ButtonUrl = "/Demo",
                        Features = "Unlimited Students per Enrolled Classroom\nAutomated Report Cards & QR Verification\nSubject Teacher Continuous Assessment Portals\nClass Broadsheet Master Downloads\nClassroom Attendance & Behavior Tracking\nIntegrated CBT Examination System\nAI Holiday Homework & Coding Mentor\nPriority 24/7 Phone & WhatsApp Support",
                        SortOrder = 2,
                        IsActive = true
                    },
                    new MasterPricingPlan
                    {
                        Name = "Annual Unlimited Institution",
                        Subtitle = "Comprehensive flat annual license covering all students, staff, and campuses.",
                        PriceText = "Custom Quote",
                        BillingCycle = "/ academic session",
                        Description = "Full enterprise license billed once per academic year with massive institutional savings.",
                        BadgeText = "Best Institutional Value",
                        IsPopular = false,
                        ButtonText = "Request Proposal",
                        ButtonUrl = "/Contact",
                        Features = "Unlimited Students, Teachers & Admin Staff\nMulti-Branch / Multi-Campus Central Dashboard\nCustom Domain & Branded School Portal\nBursary & Financial Ledger Reconciliation\nDigital Scratch Card & Result PIN Access\nHostel & Boarding House Allocation\nAI Learning & Holiday Skills Hub\nDedicated Onboarding Engineer & Staff Training",
                        SortOrder = 3,
                        IsActive = true
                    },
                    new MasterPricingPlan
                    {
                        Name = "Digital Scratch Card Model",
                        Subtitle = "Self-funding model where parents purchase result verification PIN cards per term.",
                        PriceText = "₦300 - ₦600",
                        BillingCycle = "/ scratch card PIN",
                        Description = "Zero upfront setup cost for schools; platform financed through termly result verification cards.",
                        BadgeText = "Zero Upfront Cost",
                        IsPopular = false,
                        ButtonText = "Order Scratch Cards",
                        ButtonUrl = "/Contact",
                        Features = "Zero Upfront Software Setup Fees\nCustom Branded Digital Scratch Cards & PINs\nTamper-Proof Online Result Checking Portal\nRevenue Sharing & Instant School Rebates\nAutomated SMS with Result PIN to Parents\nFree Staff Training & Portal Setup\n24/7 Card Validation Support Desk",
                        SortOrder = 4,
                        IsActive = true
                    }
                };

                await context.PricingPlans.AddRangeAsync(defaultPlans);
                await context.SaveChangesAsync();
            }
            // Seed Knowledge Base items if empty
            if (!await context.KnowledgeBaseItems.AnyAsync())
            {
                var defaultGuides = new List<MasterKnowledgeBaseItem>
                {
                    // ==========================================
                    // 1. ADMIN ROLE GUIDES
                    // ==========================================
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Academic Sessions & Term Setup",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Academic Sessions",
                        ActionName = "Index",
                        RouteUrl = "/Admin/Sessions",
                        BadgeText = "Academic Calendar",
                        SortOrder = 1,
                        IsPublished = true,
                        WhatItDoes = "Manages the institutional academic calendar, initializes new academic years (e.g. 2024/2025), and sets the active session and term for all tenant operations.",
                        Instructions = "1. Navigate to /Admin/Sessions from the Master Admin navigation.\n2. Click 'New Session' to create an upcoming academic year.\n3. Enter the session title (e.g. '2024/2025 Session') and start/end dates.\n4. Select 'Set as Current' to activate the session across all class registers and fees.\n5. Click Save to apply changes across the portal."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Term Transition & Session Rollover",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Academic Sessions",
                        ActionName = "NextTerm",
                        RouteUrl = "/Admin/Sessions/NextTerm",
                        BadgeText = "Term Rollover",
                        SortOrder = 2,
                        IsPublished = true,
                        WhatItDoes = "Advances school operations to the next term (1st Term -> 2nd Term -> 3rd Term), locking past term continuous assessment edits and initializing fresh score entry registers.",
                        Instructions = "1. Verify that all subject scores and report cards for the ending term have been approved and published.\n2. Go to /Admin/Sessions and locate the active session.\n3. Click 'Next Term' to advance the active term pointer.\n4. Review the confirmation dialog and verify the new active term.\n5. The system automatically rolls over student class enrollments while preserving historical transcripts."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Student Bio-Data & Master Profiles",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Student Profiles",
                        ActionName = "Index",
                        RouteUrl = "/Admin/StudentProfiles",
                        BadgeText = "Student Directory",
                        SortOrder = 3,
                        IsPublished = true,
                        WhatItDoes = "Maintains the central student information database, tracking official admission numbers, biometric details, guardian phone contacts, and class allocations.",
                        Instructions = "1. Navigate to /Admin/StudentProfiles to browse the complete enrolled student roster.\n2. Use the search bar to filter by Registration Number, Surname, or Class Level.\n3. Click 'Profile' on any student record to view or edit guardian contacts, residential addresses, and medical notes.\n4. Upload digital passport photographs (JPEG/PNG) to appear on report cards and student ID cards."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Class Levels, Arms & Subject Allocation",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Class Levels & Promotion",
                        ActionName = "Index",
                        RouteUrl = "/Admin/ClassLevels",
                        BadgeText = "Class Management",
                        SortOrder = 4,
                        IsPublished = true,
                        WhatItDoes = "Configures educational tiers (Nursery, Primary, Junior & Senior Secondary), class arms (e.g. JSS1 Gold, SS2 Science), and assigns curriculum subjects to each class.",
                        Instructions = "1. Open /Admin/ClassLevels.\n2. Click 'Create' to add a new class tier (e.g. 'JSS 1').\n3. Add classroom arms (e.g. 'JSS 1A', 'JSS 1B') and assign class masters/teachers.\n4. Click 'ClassSubject' to select compulsory and elective curriculum subjects offered in that arm.\n5. Save configuration to populate teacher scoring rosters."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "End-of-Session Student Promotion",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Class Levels & Promotion",
                        ActionName = "StudentPromotion",
                        RouteUrl = "/Admin/ClassLevels/StudentPromotion",
                        BadgeText = "Promotion Engine",
                        SortOrder = 5,
                        IsPublished = true,
                        WhatItDoes = "Automates the annual promotion of passing students to the subsequent class level based on cumulative annual grade averages.",
                        Instructions = "1. Open /Admin/ClassLevels/StudentPromotion at the end of the 3rd Term.\n2. Select the current class arm (e.g. JSS 1 Gold) and destination class (e.g. JSS 2 Gold).\n3. Review the student list with computed cumulative annual averages.\n4. Check students qualifying for promotion (uncheck retained or repeated pupils).\n5. Click 'Execute Promotion' to transfer active enrollments into the new class tier."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Terminal Result Processing & Broadsheets",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Result Processing & Broadsheets",
                        ActionName = "BatchResult",
                        RouteUrl = "/Admin/ResultManagement/BatchResult",
                        BadgeText = "Academics & Grading",
                        SortOrder = 6,
                        IsPublished = true,
                        WhatItDoes = "Compiles CA1, CA2, exam scores, affective & psychomotor domain ratings, class rankings, and generates cumulative broadsheets.",
                        Instructions = "1. Navigate to /Admin/ResultManagement/BatchResult.\n2. Select Session, Term, and Class Level to process.\n3. Click 'Compute Broadsheet' to calculate total scores, class averages, subject highs/lows, and position rankings.\n4. Review computational sanity checks to ensure zero missing marks.\n5. Click 'Approve & Publish' to release results to parent/student portals."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Batch Printable Report Cards",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Result Processing & Broadsheets",
                        ActionName = "StudentsForBatchResult",
                        RouteUrl = "/Admin/ResultManagement/StudentsForBatchResult",
                        BadgeText = "Report Cards",
                        SortOrder = 7,
                        IsPublished = true,
                        WhatItDoes = "Generates high-resolution, institutional-branded terminal report card PDFs ready for bulk printing with student photos, grading scales, and digital signatures.",
                        Instructions = "1. Open /Admin/ResultManagement/StudentsForBatchResult.\n2. Choose the published class arm and term.\n3. Review principal comments and class teacher remarks.\n4. Click 'Print All Terminal Cards' to generate a unified multi-page PDF formatted for A4 printing.\n5. Use 'Download Zip' to save individual student report sheets."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Scratch Card & Result-Checking PIN Generator",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Result PINs & Scratch Cards",
                        ActionName = "Index",
                        RouteUrl = "/Admin/PinManagement",
                        BadgeText = "Result PINs",
                        SortOrder = 8,
                        IsPublished = true,
                        WhatItDoes = "Generates encrypted digital scratch cards and serial PINs with access counters to monetize or secure online result checking.",
                        Instructions = "1. Navigate to /Admin/PinManagement.\n2. Click 'UpdatePinWithSessionId' to create a new batch of PINs for the active session.\n3. Specify quantity (e.g. 500 cards) and usage limit per card (e.g. 5 result views).\n4. Click 'Export to Excel' to send batch serial numbers to your commercial scratch card printer.\n5. Track active, used, and expired cards from the summary table."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "School Tuition Fees & Levies Setup",
                        TargetRole = "Admin",
                        Area = "Admin",
                        ControllerName = "Fee Schedules & Bursary",
                        ActionName = "Index",
                        RouteUrl = "/Admin/SchoolFees",
                        BadgeText = "Bursary & Fees",
                        SortOrder = 9,
                        IsPublished = true,
                        WhatItDoes = "Defines tuition fee schedules, developmental levies, PTA dues, uniform charges, and optional boarding fees per class category.",
                        Instructions = "1. Go to /Admin/SchoolFees.\n2. Click 'Create Fee Schedule' and select target Class Tier (e.g. SS1 Science).\n3. Enter itemized breakdown (Tuition, ICT, Lab Levy, PTA).\n4. Mark items as compulsory or optional.\n5. Save schedule to automatically generate student invoices on the parent portal."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Bulk SMS Broadcast to Guardians",
                        TargetRole = "Admin",
                        Area = "Content",
                        ControllerName = "Guardian Messaging",
                        ActionName = "SendToAllParents",
                        RouteUrl = "/Content/Message/SendToAllParents",
                        BadgeText = "Parent Communication",
                        SortOrder = 10,
                        IsPublished = true,
                        WhatItDoes = "Delivers real-time SMS broadcasts to all registered parent phone numbers using the school's verified custom Sender ID.",
                        Instructions = "1. Navigate to /Content/Message/SendToAllParents.\n2. Type your broadcast message (e.g. Mid-term break notification, fee reminders, emergency announcements).\n3. Preview character count and SMS credit calculation.\n4. Click 'Send Broadcast' for instant gateway delivery with live delivery logs."
                    },

                    // ==========================================
                    // 2. STAFF & TEACHER ROLE GUIDES
                    // ==========================================
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Teacher Academic Panel & Class Roster",
                        TargetRole = "Staff",
                        Area = "Staff",
                        ControllerName = "Staff Academic Panel",
                        ActionName = "Index",
                        RouteUrl = "/Staff/Panel",
                        BadgeText = "Staff Dashboard",
                        SortOrder = 11,
                        IsPublished = true,
                        WhatItDoes = "Central staff cockpit displaying assigned teaching subjects, assigned class arms, student rosters, and administrative notices.",
                        Instructions = "1. Log in with your teacher credentials.\n2. The main panel displays your assigned teaching subjects and weekly class periods.\n3. Click on any class arm to view the student name list and attendance register.\n4. Use the quick links to enter CA scores or upload homework."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Continuous Assessment (CA) Scoresheet Entry",
                        TargetRole = "Staff",
                        Area = "Staff",
                        ControllerName = "Staff Academic Panel",
                        ActionName = "ScoreSheet",
                        RouteUrl = "/Staff/Panel/ScoreSheet",
                        BadgeText = "Grade Entry",
                        SortOrder = 12,
                        IsPublished = true,
                        WhatItDoes = "Enables subject teachers to input, update, and upload continuous assessment marks (CA1, CA2) and examination scores for enrolled pupils.",
                        Instructions = "1. Navigate to /Staff/Panel/ScoreSheet.\n2. Select your Subject, Session, Term, and Class Arm.\n3. Enter CA 1 (e.g. /20), CA 2 (e.g. /20), and Examination score (e.g. /60).\n4. The system calculates the total out of 100% in real-time.\n5. Click 'Submit Scoresheet' to submit marks for administrative approval."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Digital Lesson Notes & Schemes of Work",
                        TargetRole = "Staff",
                        Area = "Staff",
                        ControllerName = "Staff Academic Panel",
                        ActionName = "LessonNote",
                        RouteUrl = "/Staff/Panel/LessonNote",
                        BadgeText = "Lesson Notes",
                        SortOrder = 13,
                        IsPublished = true,
                        WhatItDoes = "Digital repository for preparing, submitting, and publishing weekly lesson notes and curriculum materials for student revision.",
                        Instructions = "1. Go to /Staff/Panel/LessonNote.\n2. Click 'Add Lesson' and choose your Subject and Class Level.\n3. Enter the Week Number, Topic, Behavioural Objectives, and Content Summary.\n4. Attach PDF/Word diagrams or reference documents.\n5. Click 'Submit to Principal/HOD' for review and approval."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Class Homework & Assignments Manager",
                        TargetRole = "Staff",
                        Area = "Staff",
                        ControllerName = "Staff Academic Panel",
                        ActionName = "AllAssignments",
                        RouteUrl = "/Staff/Panel/AllAssignments",
                        BadgeText = "Assignments",
                        SortOrder = 14,
                        IsPublished = true,
                        WhatItDoes = "Allows teachers to create homework tasks, attach learning resources, set submission deadlines, and review student submissions.",
                        Instructions = "1. Open /Staff/Panel/AllAssignments.\n2. Click 'Add Assignment'.\n3. Enter Assignment Title, Subject, and Class Arm.\n4. Write the homework instructions and attach supplementary worksheets.\n5. Set the due date and click 'Publish to Students'."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "CBT Objective Question Bank & Exam Creation",
                        TargetRole = "Staff",
                        Area = "CBTExam",
                        ControllerName = "CBT Question Bank",
                        ActionName = "Create",
                        RouteUrl = "/CBTExam/CBTQuestion/Create",
                        BadgeText = "CBT Lab",
                        SortOrder = 15,
                        IsPublished = true,
                        WhatItDoes = "Subject teacher tool for authoring online multiple-choice questions, adding mathematical formulas/diagrams, and setting correct answer keys.",
                        Instructions = "1. Navigate to /CBTExam/CBTQuestion/Create.\n2. Select your Subject and Class Tier.\n3. Type the question body and optionally attach a diagram or formula image.\n4. Provide Option A, Option B, Option C, Option D, and mark the correct option.\n5. Save question to add it to your school's verified CBT Question Bank."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "CBT Exam Board Scheduling & Proctoring",
                        TargetRole = "Staff",
                        Area = "CBTExam",
                        ControllerName = "CBT Exam Board",
                        ActionName = "Index",
                        RouteUrl = "/CBTExam/CBTExamBoard",
                        BadgeText = "CBT Scheduling",
                        SortOrder = 16,
                        IsPublished = true,
                        WhatItDoes = "Schedules online computer-based tests, sets duration timers, configures question randomization, and enables live test proctoring.",
                        Instructions = "1. Open /CBTExam/CBTExamBoard.\n2. Click 'Create Exam' and select the Subject and Target Class.\n3. Specify Duration (e.g. 45 minutes) and Total Questions to draw from the bank.\n4. Set the Exam Date, Start Time, and End Time.\n5. Click 'Activate Exam' when ready for students to start testing."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Daily Class Attendance & Register",
                        TargetRole = "Staff",
                        Area = "Staff",
                        ControllerName = "Attendance Register",
                        ActionName = "Index",
                        RouteUrl = "/Staff/EventsControl",
                        BadgeText = "Attendance",
                        SortOrder = 17,
                        IsPublished = true,
                        WhatItDoes = "Digital class attendance register for recording daily student presence (Present, Absent, Excused) used for terminal report card attendance totals.",
                        Instructions = "1. Go to /Staff/EventsControl.\n2. Select your Class Arm and the current Date.\n3. Mark each student's morning and afternoon attendance status.\n4. Add brief absence reasons where applicable.\n5. Save register; attendance figures automatically calculate on termly report sheets."
                    },

                    // ==========================================
                    // 3. STUDENT & PARENT ROLE GUIDES
                    // ==========================================
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Student Academic Cockpit & Profile",
                        TargetRole = "Student",
                        Area = "Student",
                        ControllerName = "Student Portal & Dashboard",
                        ActionName = "Index",
                        RouteUrl = "/Student/Panel",
                        BadgeText = "Student Dashboard",
                        SortOrder = 18,
                        IsPublished = true,
                        WhatItDoes = "Personalized student portal displaying enrolled subjects, class timetable, homework notifications, and school circulars.",
                        Instructions = "1. Log in with your Student Registration Number / Username and Password.\n2. View your active academic session, class arm, and class master details on the dashboard.\n3. Check urgent announcements and upcoming assignment deadlines.\n4. Click 'Personaltimetable' to view daily period schedules and classroom venues."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Online Result Checking with Scratch Card PIN",
                        TargetRole = "Student",
                        Area = "Student",
                        ControllerName = "Online Result Checker",
                        ActionName = "Index",
                        RouteUrl = "/Student/ArchiveResult",
                        BadgeText = "Result Checker",
                        SortOrder = 19,
                        IsPublished = true,
                        WhatItDoes = "Secure online result-checking portal allowing parents and students to view termly transcripts using a school scratch card PIN.",
                        Instructions = "1. Navigate to /Student/ArchiveResult.\n2. Select the Academic Session and Term (e.g. 2024/2025 Second Term).\n3. Enter your Serial Number and 12-digit Scratch Card PIN.\n4. Click 'Check Result' to verify credentials.\n5. Instantly view continuous assessment breakdown, subject grades, positions, and principal remarks."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Download Official Printable Report Card",
                        TargetRole = "Student",
                        Area = "Student",
                        ControllerName = "Online Result Checker",
                        ActionName = "PrintResult",
                        RouteUrl = "/Student/ArchiveResult/PrintResult",
                        BadgeText = "PDF Transcript",
                        SortOrder = 20,
                        IsPublished = true,
                        WhatItDoes = "Generates the authenticated, official digital terminal report card featuring school logo, student photo, grading scale, and digital verification seal.",
                        Instructions = "1. After checking your result in /Student/ArchiveResult, click 'Print Report Card'.\n2. The system renders the complete high-resolution terminal transcript.\n3. Click 'Download PDF' or 'Print' to save or print on A4 paper for records.\n4. The report card includes the verifiable QR/barcode for institutional validation."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Taking Online CBT Computer-Based Examinations",
                        TargetRole = "Student",
                        Area = "CBTExam",
                        ControllerName = "CBT Exam Board",
                        ActionName = "Students",
                        RouteUrl = "/CBTExam/CBTExamBoard/Students",
                        BadgeText = "CBT Testing",
                        SortOrder = 21,
                        IsPublished = true,
                        WhatItDoes = "Interactive student computer-based testing interface with countdown timer, question navigator, and instant submission.",
                        Instructions = "1. Go to the CBT Exam section on your student panel when a scheduled test is open.\n2. Click 'Start Exam' to load the test interface.\n3. Read each question carefully and select your chosen radio option (A, B, C, D).\n4. Use the bottom question palette to jump between answered and unanswered questions.\n5. Click 'Submit Exam' before the live countdown timer reaches 00:00."
                    },
                    new MasterKnowledgeBaseItem
                    {
                        Title = "Downloading Homework & Lesson Notes",
                        TargetRole = "Student",
                        Area = "Student",
                        ControllerName = "Student Portal & Dashboard",
                        ActionName = "Assignment",
                        RouteUrl = "/Student/Panel/Assignment",
                        BadgeText = "E-Learning",
                        SortOrder = 22,
                        IsPublished = true,
                        WhatItDoes = "Enables students to download subject homework tasks, view teacher notes, and study course syllabi from home on desktop or mobile.",
                        Instructions = "1. Open /Student/Panel/Assignment from your student dashboard.\n2. View active assignments grouped by subject and deadline date.\n3. Click 'Details' to view teacher instructions and download attached worksheets.\n4. Go to /Student/Panel/Sylabus to read digital lesson notes and prepare ahead for classes."
                    }
                };

                await context.KnowledgeBaseItems.AddRangeAsync(defaultGuides);
                await context.SaveChangesAsync();
            }

        }

        private static async Task SeedIdentityAndFinancialsAsync(MasterDbContext context, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetService<UserManager<MasterAdminUser>>();

            if (roleManager != null && userManager != null)
            {
                // 1. Seed Roles
                foreach (var roleName in MasterRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // 2. Seed Default SuperAdmin from .env
                var adminEmail = Environment.GetEnvironmentVariable("MASTER_ADMIN_EMAIL");
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    adminEmail = "superadmin@exwhyzee.ng";
                }

                var adminPassword = Environment.GetEnvironmentVariable("MASTER_ADMIN_DEFAULT_PASSWORD");
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    adminPassword = "ExwhyzeeAdmin2026!";
                }

                var existingAdmin = await userManager.FindByEmailAsync(adminEmail) ?? await userManager.FindByNameAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var superAdminUser = new MasterAdminUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Master Super Administrator",
                        RoleTitle = MasterRoles.SuperAdmin,
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(superAdminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdminUser, MasterRoles.SuperAdmin);
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(existingAdmin, MasterRoles.SuperAdmin))
                    {
                        await userManager.AddToRoleAsync(existingAdmin, MasterRoles.SuperAdmin);
                    }
                }
            }

            // 3. Seed Initial School Billing & Transactions if empty
            if (!await context.SchoolTransactions.AnyAsync())
            {
                var premierSchool = await context.TenantSchools.FirstOrDefaultAsync(s => s.SubDomain == "premier") 
                                 ?? await context.TenantSchools.FirstOrDefaultAsync();

                if (premierSchool != null)
                {
                    var demoTransactions = new List<MasterSchoolTransaction>
                    {
                        new MasterSchoolTransaction
                        {
                            TenantSchoolId = premierSchool.Id,
                            InvoiceNumber = "INV-2026-00101",
                            Title = $"{premierSchool.SchoolName} - 2026/2027 1st Term Portal License & Support",
                            BillingType = SchoolBillingType.TermlySubscription,
                            Amount = 180000m,
                            AmountPaid = 180000m,
                            Status = SchoolPaymentStatus.Paid,
                            PaymentChannel = SchoolPaymentChannel.BankTransfer,
                            BankReference = "GTB/TRF/9834710294",
                            DueDate = DateTime.UtcNow.AddDays(-15),
                            PaidDate = DateTime.UtcNow.AddDays(-14),
                            ReceiptNumber = "REC-2026-00088",
                            Description = "Full termly subscription coverage for 450 enrolled students.",
                            RecordedBy = "Master Billing Engine",
                            CreatedAt = DateTime.UtcNow.AddDays(-20)
                        },
                        new MasterSchoolTransaction
                        {
                            TenantSchoolId = premierSchool.Id,
                            InvoiceNumber = "INV-2026-00102",
                            Title = "5,000 Scratch Cards / Result Verification PIN Batches",
                            BillingType = SchoolBillingType.ResultPINs,
                            Amount = 250000m,
                            AmountPaid = 250000m,
                            Status = SchoolPaymentStatus.Paid,
                            PaymentChannel = SchoolPaymentChannel.Paystack,
                            BankReference = "PSTK_8471092837",
                            DueDate = DateTime.UtcNow.AddDays(-5),
                            PaidDate = DateTime.UtcNow.AddDays(-4),
                            ReceiptNumber = "REC-2026-00095",
                            Description = "Automated result checker serial pins batch 1 to 5,000.",
                            RecordedBy = "Master Billing Engine",
                            CreatedAt = DateTime.UtcNow.AddDays(-6)
                        },
                        new MasterSchoolTransaction
                        {
                            TenantSchoolId = premierSchool.Id,
                            InvoiceNumber = "INV-2026-00103",
                            Title = $"{premierSchool.SchoolName} - 2026/2027 2nd Term Renewal Subscription",
                            BillingType = SchoolBillingType.TermlySubscription,
                            Amount = 180000m,
                            AmountPaid = 0m,
                            Status = SchoolPaymentStatus.Pending,
                            PaymentChannel = SchoolPaymentChannel.BankTransfer,
                            DueDate = DateTime.UtcNow.AddDays(25),
                            Description = "Termly renewal invoice issued for next term resumption.",
                            RecordedBy = "Master Billing Engine",
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.SchoolTransactions.AddRangeAsync(demoTransactions);
                    await context.SaveChangesAsync();
                }
            }

            // 4. Seed Tenant Schools if none exist
            if (!await context.TenantSchools.AnyAsync())
            {
                var defaultDbConn = Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION") ?? string.Empty;

                var demoSchools = new List<TenantSchool>
                {
                    new TenantSchool
                    {
                        SchoolName = "Premier International Academy",
                        SubDomain = "premier",
                        HostDomain = "portal.premieracademy.edu.ng",
                        ConnectionString = defaultDbConn,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow.AddMonths(-8)
                    },
                    new TenantSchool
                    {
                        SchoolName = "Grace Model College",
                        SubDomain = "gracemodel",
                        HostDomain = "portal.gracemodel.com",
                        ConnectionString = defaultDbConn,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow.AddMonths(-6)
                    },
                    new TenantSchool
                    {
                        SchoolName = "Al-Hikmah Comprehensive Academy",
                        SubDomain = "alhikmah",
                        HostDomain = null,
                        ConnectionString = defaultDbConn,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow.AddMonths(-4)
                    },
                    new TenantSchool
                    {
                        SchoolName = "St. Jude Catholic High School",
                        SubDomain = "stjude",
                        HostDomain = "portal.stjudehigh.sch.ng",
                        ConnectionString = defaultDbConn,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow.AddMonths(-2)
                    }
                };

                await context.TenantSchools.AddRangeAsync(demoSchools);
                await context.SaveChangesAsync();
            }

            // 5. Seed Statistical Telemetry Metric Snapshots if empty
            if (!await context.TenantMetricSnapshots.AnyAsync())
            {
                var allSchools = await context.TenantSchools.ToListAsync();
                var snapshots = new List<MasterTenantMetricSnapshot>();

                foreach (var school in allSchools)
                {
                    int totalStuds = school.SubDomain switch
                    {
                        "premier" => 540,
                        "gracemodel" => 420,
                        "alhikmah" => 380,
                        "stjude" => 610,
                        _ => 300
                    };

                    int maleStuds = (int)(totalStuds * 0.52);
                    int femaleStuds = totalStuds - maleStuds;

                    int earlyYears = (int)(totalStuds * 0.15);
                    int primary = (int)(totalStuds * 0.35);
                    int jss = (int)(totalStuds * 0.28);
                    int sss = totalStuds - (earlyYears + primary + jss);

                    int totalStaff = (int)(totalStuds / 12);
                    int academicStaff = (int)(totalStaff * 0.78);
                    int nonAcademicStaff = totalStaff - academicStaff;

                    decimal feePerStudent = school.SubDomain == "premier" ? 120000m : 85000m;
                    decimal totalBilled = totalStuds * feePerStudent;
                    decimal totalCollected = (decimal)((double)totalBilled * (school.SubDomain == "premier" ? 0.94 : 0.86));
                    decimal totalPending = totalBilled - totalCollected;

                    snapshots.Add(new MasterTenantMetricSnapshot
                    {
                        TenantSchoolId = school.Id,
                        SnapshotDate = DateTime.UtcNow,
                        TotalStudents = totalStuds,
                        MaleStudents = maleStuds,
                        FemaleStudents = femaleStuds,
                        EarlyYearsStudents = earlyYears,
                        PrimaryStudents = primary,
                        JuniorSecondaryStudents = jss,
                        SeniorSecondaryStudents = sss,
                        TotalStaff = totalStaff,
                        AcademicStaff = academicStaff,
                        NonAcademicStaff = nonAcademicStaff,
                        TotalClasses = 18,
                        TotalSubjects = 24,
                        ActiveSession = "2026/2027",
                        ActiveTerm = "1st Term",
                        DailyAttendanceRate = school.SubDomain == "premier" ? 96.4 : 93.8,
                        TotalFeesBilled = totalBilled,
                        TotalFeesCollected = totalCollected,
                        TotalFeesPending = totalPending,
                        TotalResultsPublished = (int)(totalStuds * 0.98),
                        TotalPinsVerified = (int)(totalStuds * 1.4),
                        CbtExamsConducted = 14,
                        CbtSubmissions = totalStuds * 6,
                        StorageUsedMb = 2450.5,
                        DatabaseSizeBytes = 450000000,
                        ApiLatencyMs = 28,
                        HealthStatus = TelemetryHealthStatus.Healthy,
                        ClientVersion = "v2.5.0",
                        SyncSource = "TenantReportApi",
                        RecordedAtUtc = DateTime.UtcNow
                    });
                }

                await context.TenantMetricSnapshots.AddRangeAsync(snapshots);
                await context.SaveChangesAsync();
            }

            // 6. Seed Sample Security Audit Logs if empty
            if (!await context.AuditLogs.AnyAsync())
            {
                var sampleLogs = new List<MasterAuditLog>
                {
                    new MasterAuditLog
                    {
                        TimestampUtc = DateTime.UtcNow.AddHours(-1),
                        UserName = "superadmin@exwhyzee.ng",
                        UserRole = MasterRoles.SuperAdmin,
                        Action = "LoginSuccess",
                        Details = "Master SuperAdmin logged in successfully from secure terminal.",
                        Area = "Security",
                        Severity = "Info",
                        IpAddress = "197.210.54.12",
                        Browser = "Google Chrome",
                        OperatingSystem = "Windows 11",
                        DeviceType = "Desktop"
                    },
                    new MasterAuditLog
                    {
                        TimestampUtc = DateTime.UtcNow.AddMinutes(-35),
                        UserName = "superadmin@exwhyzee.ng",
                        UserRole = MasterRoles.SuperAdmin,
                        Action = "DelegatedSsoLaunch",
                        Details = "Master user requested support access as 'SuperAdmin' into 'Premier International Academy'. Reason: Routine database migration verification",
                        Area = "Security",
                        Severity = "Warning",
                        TenantSubDomain = "premier",
                        IpAddress = "197.210.54.12",
                        Browser = "Google Chrome",
                        OperatingSystem = "Windows 11",
                        DeviceType = "Desktop"
                    },
                    new MasterAuditLog
                    {
                        TimestampUtc = DateTime.UtcNow.AddMinutes(-34),
                        UserName = "superadmin@exwhyzee.ng",
                        UserRole = "SuperAdmin",
                        Action = "DelegatedSsoAccept",
                        Details = "Support session established on 'Premier International Academy' for Master Admin 'superadmin@exwhyzee.ng'.",
                        Area = "Tenant",
                        Severity = "Warning",
                        TenantSubDomain = "premier",
                        IpAddress = "197.210.54.12",
                        Browser = "Google Chrome",
                        OperatingSystem = "Windows 11",
                        DeviceType = "Desktop"
                    }
                };

                await context.AuditLogs.AddRangeAsync(sampleLogs);
                await context.SaveChangesAsync();
            }

            // 7. Seed Sample Email Outbox Dump Logs if empty
            if (!await context.EmailLogs.AnyAsync())
            {
                var sampleEmails = new List<MasterEmailLog>
                {
                    new MasterEmailLog
                    {
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                        RecipientEmail = "admin@premieracademy.edu.ng",
                        RecipientName = "Dr. Anthony Uche",
                        SenderEmail = "billing@exwhyzee.ng",
                        Subject = "Exwhyzee SP - Invoice INV-2026-00101 Issued (2026/2027 1st Term)",
                        EmailCategory = "BillingInvoice",
                        Status = "LoggedAndDumped",
                        TriggeredBy = "Master Billing Engine",
                        TenantSubDomain = "premier",
                        BodyHtml = @"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 24px;'>
                            <h2 style='color: #1e3a8a; margin-top: 0;'>Premier International Academy</h2>
                            <p>Dear Dr. Anthony Uche,</p>
                            <p>Your termly portal subscription invoice <strong>INV-2026-00101</strong> for <strong>₦180,000.00</strong> has been generated.</p>
                            <div style='background: #f8fafc; padding: 16px; border-radius: 6px; margin: 20px 0;'>
                                <div><strong>Invoice Number:</strong> INV-2026-00101</div>
                                <div><strong>Coverage:</strong> 2026/2027 1st Term Portal License &amp; Support</div>
                                <div><strong>Amount:</strong> ₦180,000.00</div>
                                <div><strong>Status:</strong> <span style='color: #16a34a; font-weight: bold;'>PAID</span></div>
                            </div>
                            <p>Thank you for choosing Exwhyzee School Portal.</p>
                            <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                            <small style='color: #64748b;'>Exwhyzee SP Multi-Tenant Platform &bull; Automated Billing System</small>
                        </div>"
                    },
                    new MasterEmailLog
                    {
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-5),
                        RecipientEmail = "proprietor@gracemodel.com",
                        RecipientName = "Mrs. Folashade Adeleke",
                        SenderEmail = "onboarding@exwhyzee.ng",
                        Subject = "Welcome to Exwhyzee SP - Grace Model College Portal Activated",
                        EmailCategory = "Onboarding",
                        Status = "LoggedAndDumped",
                        TriggeredBy = "Tenant Provisioning Service",
                        TenantSubDomain = "gracemodel",
                        BodyHtml = @"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 24px;'>
                            <h2 style='color: #0f766e; margin-top: 0;'>Welcome to Exwhyzee School Portal</h2>
                            <p>Dear Mrs. Folashade Adeleke,</p>
                            <p>We are excited to inform you that the digital portal for <strong>Grace Model College</strong> is now fully provisioned and ready.</p>
                            <p><strong>Your Portal URL:</strong> <a href='https://gracemodel.exwhyzee.ng'>https://gracemodel.exwhyzee.ng</a></p>
                            <p>Log in with your administrator credentials to set up class levels, staff profiles, and student rosters.</p>
                            <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;' />
                            <small style='color: #64748b;'>Exwhyzee SP &bull; Customer Success &amp; Onboarding Team</small>
                        </div>"
                    }
                };

                await context.EmailLogs.AddRangeAsync(sampleEmails);
                await context.SaveChangesAsync();
            }

            // 8. Seed Marketing Templates if empty
            if (!await context.MarketingTemplates.AnyAsync())
            {
                var templates = new List<MarketingTemplate>
                {
                    new MarketingTemplate
                    {
                        TemplateName = "Full School ERP & Portal Showcase",
                        Subject = "Transform {{SchoolName}} with All-in-One Cloud Management & Automated Broadsheets",
                        PreheaderText = "Automate report cards, eliminate result calculation errors, and collect fees with 99% efficiency.",
                        Category = "ProductPitch",
                        Description = "Comprehensive introduction to Exwhyzee SP featuring automated report cards, digital CBT exams, online admissions, and fee tracking.",
                        IsDefault = true,
                        CreatedAtUtc = DateTime.UtcNow,
                        BodyHtml = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
  body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #f1f5f9; margin: 0; padding: 20px; color: #1e293b; }
  .card { max-width: 620px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.06); border: 1px solid #e2e8f0; }
  .header { background: linear-gradient(135deg, #0b2239 0%, #173b61 100%); padding: 36px 30px; text-align: center; color: #ffffff; }
  .header h1 { margin: 0; font-size: 24px; font-weight: 800; letter-spacing: -0.5px; }
  .header p { margin: 8px 0 0; font-size: 14px; opacity: 0.9; color: #38bdf8; }
  .content { padding: 32px 30px; line-height: 1.65; font-size: 15px; }
  .highlight-box { background: #f8fafc; border-left: 4px solid #0284c7; padding: 18px; border-radius: 0 8px 8px 0; margin: 24px 0; }
  .grid-feature { display: table; width: 100%; margin: 20px 0; }
  .feature-col { display: table-cell; width: 50%; padding: 10px; vertical-align: top; }
  .feature-item { background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 8px; padding: 14px; }
  .feature-item h4 { margin: 0 0 6px; color: #166534; font-size: 14px; }
  .feature-item p { margin: 0; font-size: 13px; color: #374151; line-height: 1.4; }
  .btn-wrap { text-align: center; margin: 30px 0 20px; }
  .btn { display: inline-block; background: #0284c7; color: #ffffff !important; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 700; font-size: 15px; box-shadow: 0 4px 10px rgba(2,132,199,0.3); }
  .footer { background: #0f172a; padding: 24px 30px; text-align: center; font-size: 12px; color: #94a3b8; }
  .footer a { color: #38bdf8; text-decoration: none; }
</style>
</head>
<body>
<div class='card'>
  <div class='header'>
    <h1>Exwhyzee School Portal</h1>
    <p>Empowering 21st Century Academic Excellence</p>
  </div>
  <div class='content'>
    <p>Dear <strong>{{FullName}}</strong>,</p>
    <p>As an educational leader at <strong>{{SchoolName}}</strong>, you understand the enormous administrative burden that comes with termly result computations, student record tracking, and fee reconciliation.</p>
    
    <div class='highlight-box'>
      <strong>Why Top Schools Across {{State}} Choose Exwhyzee SP:</strong>
      <p style='margin: 6px 0 0;'>Our cloud platform replaces tedious manual paperwork with an elegant, unified portal that connects administrators, teachers, parents, and students seamlessly.</p>
    </div>

    <div class='grid-feature'>
      <div class='feature-col'>
        <div class='feature-item'>
          <h4>&bull; Automated Report Cards</h4>
          <p>Instant grading, position computation, cognitive domain rating, and tamper-proof scratch-card PIN verification.</p>
        </div>
      </div>
      <div class='feature-col'>
        <div class='feature-item' style='background: #eff6ff; border-color: #bfdbfe;'>
          <h4 style='color: #1e40af;'>&bull; Integrated CBT Engine</h4>
          <p>Prepare students for JAMB, WAEC &amp; NECO with timed computer exams, audio questions, and real-time scores.</p>
        </div>
      </div>
    </div>

    <p>We would be thrilled to provide you with a complimentary live demo and trial instance customized for <strong>{{SchoolName}}</strong>.</p>

    <div class='btn-wrap'>
      <a href='https://exwhyzee.ng/Demo' class='btn'>Book Free Institutional Demo</a>
    </div>
    
    <p style='margin-top: 24px; font-size: 14px; color: #64748b;'>Best regards,<br><strong>Exwhyzee Academic Success Team</strong><br>Email: info@exwhyzee.ng | Tel: +234 800 EXWHYZEE</p>
  </div>
  <div class='footer'>
    <p>&copy; {{CurrentYear}} Exwhyzee Technologies Ltd. All rights reserved.</p>
    <p>You received this email because you are registered as {{Role}} at {{SchoolName}}.</p>
  </div>
</div>
</body>
</html>"
                    },
                    new MarketingTemplate
                    {
                        TemplateName = "Termly Early-Bird License Renewal Offer",
                        Subject = "Exclusive 20% Early-Bird Portal Renewal for {{SchoolName}}",
                        PreheaderText = "Lock in special discounted rates for your upcoming term portal licensing and SMS units.",
                        Category = "SpecialOffer",
                        Description = "Early renewal promotional campaign with incentive discounts for school proprietors and bursars.",
                        IsDefault = false,
                        CreatedAtUtc = DateTime.UtcNow,
                        BodyHtml = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
  body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #f8fafc; margin: 0; padding: 20px; color: #1e293b; }
  .card { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; border: 1px solid #e2e8f0; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
  .banner { background: linear-gradient(135deg, #15803d 0%, #166534 100%); padding: 32px 24px; text-align: center; color: #ffffff; }
  .banner h2 { margin: 0; font-size: 22px; font-weight: 800; }
  .badge { display: inline-block; background: #fef08a; color: #854d0e; font-weight: 800; font-size: 13px; padding: 6px 16px; border-radius: 20px; margin-top: 10px; }
  .body-content { padding: 30px 24px; font-size: 15px; line-height: 1.6; }
  .offer-card { background: #f0fdf4; border: 2px dashed #22c55e; border-radius: 10px; padding: 20px; text-align: center; margin: 24px 0; }
  .offer-card h3 { margin: 0; color: #15803d; font-size: 20px; }
  .btn { display: inline-block; background: #16a34a; color: #ffffff !important; padding: 12px 28px; border-radius: 6px; text-decoration: none; font-weight: bold; margin-top: 16px; }
  .footer { background: #0f172a; padding: 20px; text-align: center; color: #94a3b8; font-size: 12px; }
</style>
</head>
<body>
<div class='card'>
  <div class='banner'>
    <h2>Termly Portal Renewal &bull; Early Access</h2>
    <div class='badge'>20% DISCOUNT APPLIED</div>
  </div>
  <div class='body-content'>
    <p>Dear <strong>{{FullName}}</strong>,</p>
    <p>As we prepare for the upcoming academic term at <strong>{{SchoolName}}</strong>, we want to ensure zero interruption to your student portals, result computing, and SMS notification gateways.</p>
    
    <div class='offer-card'>
      <h3>Renew Before Term Resumption</h3>
      <p style='margin: 8px 0 0; color: #374151;'>Get <strong>20% discount</strong> on your student licensing tier + <strong>500 FREE branded SMS units</strong> when you renew this week.</p>
      <a href='https://exwhyzee.ng/Billing' class='btn'>Claim Early-Bird Renewal</a>
    </div>

    <p>Our dedicated support team is available 24/7 to assist with session rollover, class promotion, and fee schedule updates.</p>
    <p>Warm regards,<br><strong>Exwhyzee Accounts &amp; Success Desk</strong></p>
  </div>
  <div class='footer'>
    <p>&copy; {{CurrentYear}} Exwhyzee Technologies Ltd &bull; Lagos &bull; Abuja &bull; Enugu</p>
  </div>
</div>
</body>
</html>"
                    },
                    new MarketingTemplate
                    {
                        TemplateName = "Digital CBT & Online Exam Modernization",
                        Subject = "Supercharge Exam Prep at {{SchoolName}} with CBT Testing Engine",
                        PreheaderText = "Equip students with computer testing experience matching JAMB, WAEC and NECO standards.",
                        Category = "Announcement",
                        Description = "Targeted pitch focused on the computer-based testing engine, question bank management, and real-time grading.",
                        IsDefault = false,
                        CreatedAtUtc = DateTime.UtcNow,
                        BodyHtml = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
  body { font-family: 'Segoe UI', Arial, sans-serif; background-color: #f1f5f9; margin: 0; padding: 20px; color: #0f172a; }
  .card { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; border: 1px solid #cbd5e1; }
  .top-bar { background: #4338ca; padding: 30px; text-align: center; color: #ffffff; }
  .top-bar h2 { margin: 0; font-size: 22px; font-weight: 800; }
  .p-body { padding: 28px; line-height: 1.6; font-size: 15px; }
  .feature-pill { background: #e0e7ff; color: #3730a3; padding: 6px 12px; border-radius: 6px; font-weight: 600; display: inline-block; margin-bottom: 12px; }
  .btn-cbt { display: inline-block; background: #4338ca; color: #ffffff !important; padding: 12px 26px; border-radius: 6px; text-decoration: none; font-weight: bold; }
  .footer { background: #1e1b4b; padding: 20px; text-align: center; color: #a5b4fc; font-size: 12px; }
</style>
</head>
<body>
<div class='card'>
  <div class='top-bar'>
    <h2>Exwhyzee CBT Examination Suite</h2>
  </div>
  <div class='p-body'>
    <div class='feature-pill'>Exam Technology for Modern Schools</div>
    <p>Hello <strong>{{FullName}}</strong>,</p>
    <p>Are your students at <strong>{{SchoolName}}</strong> fully confident taking timed digital assessments on computers and tablets?</p>
    <p>Exwhyzee SP includes a built-in <strong>CBT Engine</strong> that allows your teachers to:</p>
    <ul>
      <li>Upload thousands of questions with mathematical formulas &amp; diagrams</li>
      <li>Set timed exams with randomized question order to eliminate malpractice</li>
      <li>Deliver instant automated grading and subject diagnostic reports</li>
    </ul>
    <div style='text-align: center; margin: 25px 0;'>
      <a href='https://exwhyzee.ng/CbtDemo' class='btn-cbt'>Experience CBT Test Simulation</a>
    </div>
    <p>Empower your students with the digital confidence they need for WAEC and JAMB UTME success.</p>
  </div>
  <div class='footer'>
    <p>&copy; {{CurrentYear}} Exwhyzee Technologies Ltd &bull; Dedicated Academic Solutions</p>
  </div>
</div>
</body>
</html>"
                    }
                };

                await context.MarketingTemplates.AddRangeAsync(templates);
                await context.SaveChangesAsync();
            }

            // 9. Seed Sample Marketing Contacts if empty
            if (!await context.MarketingContacts.AnyAsync())
            {
                var sampleContacts = new List<MarketingContact>
                {
                    new MarketingContact
                    {
                        Email = "proprietor@kingswayacademy.org",
                        FullName = "Chief Oladipo Johnson",
                        PhoneNumber = "08034567891",
                        SchoolName = "Kingsway International Academy",
                        RoleTitle = "Proprietor",
                        State = "Lagos",
                        Tag = "Lagos Schools",
                        Source = "ExcelImport",
                        IsSubscribed = true,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
                    },
                    new MarketingContact
                    {
                        Email = "principal@capitalhigh.edu.ng",
                        FullName = "Dr. Amina Abubakar",
                        PhoneNumber = "08023456782",
                        SchoolName = "Capital Heights Model College",
                        RoleTitle = "Principal",
                        State = "Abuja FCT",
                        Tag = "Abuja Leads",
                        Source = "ExcelImport",
                        IsSubscribed = true,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-8)
                    },
                    new MarketingContact
                    {
                        Email = "director@heritagegrammar.com",
                        FullName = "Rev. Emmanuel Nwosu",
                        PhoneNumber = "08098765432",
                        SchoolName = "Heritage Grammar School",
                        RoleTitle = "Director of Academics",
                        State = "Enugu",
                        Tag = "Prospects",
                        Source = "WordPaste",
                        IsSubscribed = true,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
                    },
                    new MarketingContact
                    {
                        Email = "bursar@beaconacademy.ng",
                        FullName = "Mrs. Kehinde Adeleke",
                        PhoneNumber = "08123456789",
                        SchoolName = "Beacon Hill Academy",
                        RoleTitle = "Bursar & Accountant",
                        State = "Ogun",
                        Tag = "Prospects",
                        Source = "Manual",
                        IsSubscribed = true,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                    }
                };

                await context.MarketingContacts.AddRangeAsync(sampleContacts);
                await context.SaveChangesAsync();
            }
        }
    }
}

