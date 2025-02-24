# **Messaging App**

Простой веб-сервис для обмена сообщениями с использованием **ASP.NET Core**, **PostgreSQL**, **WebSockets** и **Docker**.

## **Запуск проекта**

### 1. **Клонируйте репозиторий**

```bash
git clone https://github.com/isomadinow/MessagingApp.git
cd MessagingApp
```

### 2. **Запуск через Docker Compose**

Убедитесь, что у вас установлен **Docker** и **Docker Compose**. Затем выполните команду для сборки и запуска контейнеров:

```bash
docker-compose up --build
```

### 3. **Swagger**

После запуска приложения откройте Swagger UI для тестирования API:

```
http://localhost:5050/swagger/index.html
```

### 4. **WebSocket**

Чтобы получать сообщения в реальном времени, подключитесь к WebSocket-серверу:

```
ws://localhost:5050/ws
```

### 5. **Health Check**

Для проверки состояния сервиса:

```
GET http://localhost:5050/health
```

## **Структура проекта**

- **API (Controller)** — обработка HTTP-запросов (REST API).
- **Service** — логика обработки сообщений и WebSocket.
- **Repository** — доступ к данным (PostgreSQL).
- **Model** — модели данных (Message, DTO и т.д.).

## **API**

1. **Создание сообщения (POST)**:
   - **URL**: `/api/message`
   - **Тело запроса**:
     ```json
     {
       "Text": "Hello, World!",
       "MessageNumber": 1
     }
     ```

2. **Получение сообщений за период (GET)**:
   - **URL**: `/api/message?start=2025-02-01T00:00:00Z&end=2025-02-05T00:00:00Z`
   - **Ответ**:
     ```json
     [
       {
         "Id": 1,
         "Text": "Hello, World!",
         "Timestamp": "2025-02-05T00:00:00Z",
         "MessageNumber": 1
       }
     ]
     ```

