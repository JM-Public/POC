-- Create Table Locations
CREATE TABLE Locations (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Active BOOLEAN
);

-- Seed Data
INSERT INTO Locations (Name, Active) VALUES
    ('France', true),
    ('Germany', true),
    ('Italy', true),
    ('Spain', true),
    ('United Kingdom', true),
    ('United States', true),
    ('Canada', true),
    ('Mexico', true),
    ('China', false),
    ('Japan', false),
    ('South Korea', false),
    ('India', false),
    ('Thailand', false),
    ('Indonesia', false),
    ('Australia', false),
    ('Brazil', false),
    ('Argentina', false),
    ('Chile', false),
    ('Russia', false),
    ('Turkey', false);