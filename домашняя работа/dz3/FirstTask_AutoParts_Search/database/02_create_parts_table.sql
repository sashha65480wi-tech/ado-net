-- После создания autoparts_search откройте Query Tool именно для этой базы
-- и выполните этот скрипт.
CREATE TABLE IF NOT EXISTS parts (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    article TEXT NOT NULL,
    price NUMERIC(12, 2) NOT NULL CHECK (price >= 0),
    stock INTEGER NOT NULL DEFAULT 0 CHECK (stock >= 0)
);

INSERT INTO parts (name, article, price, stock)
VALUES
    ('Масляный фильтр', 'OF-1001', 450.00, 25),
    ('Воздушный фильтр', 'AF-2002', 620.00, 18),
    ('Тормозные колодки', 'BP-3003', 2100.00, 12)
ON CONFLICT DO NOTHING;

SELECT * FROM parts ORDER BY id;
