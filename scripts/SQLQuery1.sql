CREATE TABLE ingredient (
    Id INT PRIMARY KEY,
    Name VARCHAR(50),
    Amount DECIMAL(10,2),
    Unit VARCHAR(20)
);




INSERT INTO ingredient (Id, Name, Amount, Unit)
VALUES
    (1, 'Chicken breast', 500, 'g'),
    (2, 'Lemon juice', 2, 'tbsp'),
    (3, 'Olive oil', 3, 'tbsp'),
    (4, 'Garlic', 3, 'cloves'),
    (5, 'Salt', 1, 'tsp'),
    (6, 'Black pepper', 1, 'tsp'),
    (7, 'Honey', 1, 'tbsp'),
    (8, 'Soy sauce', 1, 'tbsp'),
    (9, 'Cornstarch', 2, 'tsp'),
    (10, 'Water', 1/4, 'cup');