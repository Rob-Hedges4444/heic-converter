# Regulated deployment checklist

**Owner:** ____________________  
**Release version / hash:** ____________________  
**Approval date:** ____________________

No item below is satisfied merely because the application builds.

## Before production

- [ ] Information Security confirms this component is in scope for the firm's ISMS risk assessment and Statement of Applicability, with risks, owners, treatment dates, and residual-risk approval recorded.
- [ ] Compliance confirms whether this workflow supports an important business service, and—if it does—maps its people, process, technology, information, and third-party dependencies; defines an impact tolerance; and tests a severe-but-plausible failure/manual workaround.
- [ ] Data Protection completes or records the decision not to require a DPIA; confirms lawful basis, retention, access, and data-subject handling.
- [ ] `ManagedOutputDirectory` is an IT-managed, encrypted, access-controlled location. Permission tests show a standard user can write only where authorised.
- [ ] The output location's retention schedule and secure deletion process are documented. Source-file handling is included in the same process.
- [ ] A release is produced in a controlled build pipeline, dependency versions are locked and reviewed, vulnerability scanning is clean/accepted, and the exact release is code-signed.
- [ ] Security testing covers malformed files, oversized files, unsupported formats, duplicate names, loss of output share, no disk space, denied access, and a compromised/malicious HEIC sample in a quarantined test environment.
- [ ] User acceptance testing proves images retain the required visual quality and correct orientation; business owners confirm that removed EXIF metadata is appropriate.
- [ ] The software is deployed only through the approved endpoint-management process and its publisher/signature are allow-listed.
- [ ] The service owner has an incident route, support runbook, version rollback process, and a tested manual conversion workaround.

## Configuration

In the published application directory, set the approved output location:

```json
{
  "ManagedOutputDirectory": "\\\\approved-file-server\\Claims\\HEIC-Conversions"
}
```

The application deliberately disables conversion when this value is empty, malformed, unavailable, or cannot be created/accessed. Maintain this file through the deployment tool, not by asking individual users to edit it.

## Periodic evidence

- [ ] Review access permissions and retention at least as often as the firm’s policy requires.
- [ ] Review dependencies, vulnerabilities, signing certificate status, and endpoint deployment health.
- [ ] Re-test recovery and the manual workaround at the frequency set by the operational-resilience owner.
- [ ] Reassess after a material change: library update, output-store change, claims-system integration, security incident, or new data category.
