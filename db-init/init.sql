CREATE TABLE IF NOT EXISTS messages (
    id SERIAL PRIMARY KEY,
    text VARCHAR(128) NOT NULL,
    timestamp TIMESTAMP NOT NULL,
    message_number INTEGER NOT NULL
);
