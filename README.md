
## 🏗️ System Architecture & Workflow

We designed Elixir with scalability, decoupling, and high performance in mind, utilizing an **Event-Driven Micro-Architecture** within a monolithic boundary.

```mermaid
graph TD;
    Client[📱 Flutter App] -->|REST API / JWT| API[⚙️ ASP.NET Core API]
    Client <-->|WebSockets| SignalR[🔄 SignalR Hub]
    
    API -->|Read/Write| SQL[(🗄️ SQL Server)]
    API -->|GEOSEARCH / Cache| Redis[(⚡ Redis)]
    
    API -.->|Publish Events| RMQ((🐇 RabbitMQ))
    
    RMQ -.->|Consume| Worker1[🔔 Notification Worker]
    RMQ -.->|Consume| Worker2[📍 Location Updater]
    RMQ -.->|Consume| Worker3[📊 Request State Manager]
    
    Worker1 -->|Push| FCM[🔥 Firebase Cloud Messaging]
    Worker3 -->|Broadcast| SignalR
