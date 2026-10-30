SHOP APP — запуск на Windows / macOS / Linux

Что нужно установить:
1. Docker Desktop (Windows/macOS) или Docker Engine + Docker Compose (Linux).
2. Для запуска только через Docker .NET SDK устанавливать не обязательно.

Запуск:
1. Распакуйте архив в удобную папку.
2. Откройте эту папку в терминале (PowerShell / Terminal).
3. Выполните:
   docker compose up --build

4. Дождитесь сообщений о запуске контейнеров.
5. Откройте в браузере:
   http://localhost:8080

В приложении уже будут тестовые товары. Можно:
- просматривать каталог;
- открывать карточку товара;
- добавлять товары;
- покупать товар (уменьшать остаток).

Полезные команды:
Остановить контейнеры:
   docker compose down

Посмотреть логи:
   docker compose logs -f web
   docker compose logs -f db

Посмотреть товары в PostgreSQL:
   docker compose exec db psql -U shopuser -d shopdb -c "SELECT * FROM products;"

Полностью удалить контейнеры И данные базы:
   docker compose down -v

Порты:
- сайт: http://localhost:8080
- PostgreSQL с компьютера: localhost:5434
- PostgreSQL внутри Docker-сети: db:5432

Примечание:
Если порт 8080 или 5434 уже занят, измените левую часть соответствующей
строки ports в docker-compose.yml. Например "8081:8080", тогда сайт будет
доступен по адресу http://localhost:8081.

Файл appsettings.json использует localhost:5434 для запуска приложения
на компьютере вне Docker. В контейнере строка подключения переопределяется
на Host=db;Port=5432.
