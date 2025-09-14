# AICalendarSolution       # AICalendar API
## Overview

AICalendar is a .NET 8-based RESTful API for calendar, event, attendee, and reminder management. This document provides a comprehensive overview of the API, its contract, usage, migration notes, and operational guidelines.

---

## Chosen API Style and Justification

**Style:** RESTful, resource-oriented, using standard HTTP verbs and predictable resource URLs.

**Justification:**  
REST is widely adopted, easy to consume, and aligns with OpenAPI tooling. It enables clear separation of resources, stateless operations, and is well-supported by .NET and client SDK generators.

---

## Contract Overview

- **OpenAPI Spec:** [contracts/openapi.yml](AICalendar/Contracts/openapi.yml)
- **Base URL (local):** `http://localhost:5000/v1`
- **Base URL (prod):** `https://api.aicalendar.com/v1`

**Resources:**
- `/calendars`
- `/events`
- `/attendees`
- `/reminders`

---

## Example API Calls

### Create a Calendar

curl -X POST http://localhost:5000/v1/calendars 
-H "Content-Type: application/json" 
-d '{"name":"Work","timezone":"UTC"}'
### List Events
curl http://localhost:5000/v1/events?start_time=2025-09-01T00:00:00Z&end_time=2025-09-30T23:59:59Z


### Add Attendee
curl -X POST http://localhost:5000/v1/attendees 
-H "Content-Type: application/json" 
-d '{"event_id":"<event-uuid>","email":"user@example.com","name":"User"}'


---

## Versioning and Deprecation Policy

- **Current Version:** v1 (all endpoints under `/v1/`)
- **Deprecation:**  
  - Old endpoints are removed; no deprecated endpoints remain.
  - Future breaking changes will be introduced under `/v2/` or higher.
  - Deprecated endpoints will be marked in the README and OpenAPI spec with a removal timeline.

---

## Migration Notes for Clients

- **Timeline:**  
  - All clients must migrate to `/v1/` endpoints by **2025-09-30**.
  - Old endpoints are no longer available.
- **Required Actions:**  
  - Update base URLs to `/v1/`.
  - Update request/response payloads to match the [OpenAPI contract](AICalendar/Contracts/openapi.yml).

---

## Local Development Setup and Run Instructions

1. **Clone the repository:**
2. git clone https://github.com/azaan786-0ss/AICalendarSolution.git cd AICalendarSolution/AICalendar

 **Configure the database:**
   - Update `appsettings.json` with your SQL Server connection string.

3. **Apply migrations:**

   
4. **Run the API:**

   - The API will be available at `http://localhost:5000/v1`.

5. **Swagger UI:**  
   - Visit `http://localhost:5000/swagger` for interactive API docs.

---

## Security, Performance, and Observability Notes

- **Security:**  
  - JWT-based authentication is enforced on all endpoints except reminders.
  - Input validation is centralized and returns structured errors.
- **Performance:**  
  - IP rate limiting is enabled via `AspNetCoreRateLimit`.
- **Observability:**  
  - Structured logging is implemented for all requests and errors.
  - Unhandled exceptions are returned as normalized JSON.

---

## Known Limitations

- Only basic CRUD operations are supported.
- No support for recurring events or attendee invitations.
- Timezone handling is basic; ensure all times are in ISO 8601 format.
- No webhooks or push notifications.
- No UI; API only.

---

## Day 2 Section

This section documents the post-launch operational, migration, and support policies for the AICalendar API.

- **API style:** RESTful, justified by interoperability and tooling support.
- **Contract:** [OpenAPI spec](AICalendar/Contracts/openapi.yml) governs all requests/responses.
- **Examples:** See above for copy-pastable curl commands.
- **Versioning:** v1 is current; future versions will follow semantic versioning and clear deprecation timelines.
- **Migration:** All clients must use `/v1/` endpoints by **2025-09-30**.
- **Local setup:** See instructions above.
- **Security/performance/observability:** JWT, rate limiting, structured logging, and error normalization are all implemented.
- **Limitations:** See above.

---

## Acceptance Criteria Checklist

- [x] README contains a complete “Day 2” section
- [x] Copy-pastable examples included
- [x] Migration timeline clearly documented

---

Home Work 3  

Potential Benefits for the AI Calendar Project
•	Natural Language Processing: Enable users to create, update, or query events using natural language.
•	Smart Suggestions: Suggest event times, locations, or attendees based on context.
•	Summarization: Summarize meeting notes or event details.
•	Conversational UI: Power chatbots or virtual assistants for calendar management.
•	Customization: Fine-tune Llama on your domain data for improved accuracy.
---
Summary:
•	Llama can be run locally using tools like Ollama or llama.cpp.
•	Integration with .NET is straightforward via REST API calls.
•	It can add advanced AI features to your calendar app, such as natural language event creation and smart suggestions.
