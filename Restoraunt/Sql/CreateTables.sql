CREATE TABLE Customer(
    id SERIAL PRIMARY KEY,
    fullname VARCHAR(100) NOT NULL,
    phone VARCHAR(20) UNIQUE NOT NULL
);


CREATE TABLE Menu(
    id SERIAL PRIMARY KEY,
    name_ VARCHAR(50) UNIQUE NOT NULL
);


CREATE TABLE Category(
    id SERIAL PRIMARY KEY,
    name_ VARCHAR(50) UNIQUE NOT NULL
);


CREATE TABLE Table_(
    id SERIAL PRIMARY KEY,
    num INTEGER UNIQUE NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Вільний',
    places INTEGER NOT NULL
);


CREATE TABLE Personnel(
    id SERIAL PRIMARY KEY,
    fullname VARCHAR(100) NOT NULL,
    position_ VARCHAR(50) NOT NULL,
    phone VARCHAR(20) UNIQUE NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    priority_ INTEGER NOT NULL DEFAULT 0,
    salary DECIMAL(8, 2) NOT NULL DEFAULT 50000.00
);


CREATE TABLE Dish(
    id SERIAL PRIMARY KEY,
    id_category INTEGER,
    name_ VARCHAR(50) UNIQUE NOT NULL,
    price DECIMAL(6, 2) NOT NULL CHECK(price>0.0),
    FOREIGN KEY(id_category) REFERENCES Category(id) ON DELETE SET NULL
);


CREATE TABLE Order_(
    id SERIAL PRIMARY KEY,
    id_customer INTEGER,
    id_personnel INTEGER,
    type_ VARCHAR(20) NOT NULL DEFAULT 'Замовлення',
    start_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP+INTERVAL '1 hours',
    comment TEXT DEFAULT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Нове',
    total_price DECIMAL(12, 2) NOT NULL DEFAULT 0.0,
    FOREIGN KEY(id_customer) REFERENCES Customer(id) ON DELETE SET NULL,
    FOREIGN KEY(id_personnel) REFERENCES Personnel(id) ON DELETE SET NULL
); 


CREATE TABLE Booking(
    id SERIAL PRIMARY KEY,
    id_customer INTEGER NOT NULL,
    booking_start_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    booking_end_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP+INTERVAL '2 hours',
    FOREIGN KEY(id_customer) REFERENCES Customer(id) ON DELETE CASCADE
);


CREATE TABLE TableShedule(
    id SERIAL PRIMARY KEY,
    id_table INTEGER NOT NULL,
    id_order INTEGER,
    id_booking INTEGER,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL,
    event_type VARCHAR(50),
    FOREIGN KEY(id_table) REFERENCES Table_(id) ON DELETE CASCADE,
    FOREIGN KEY(id_order) REFERENCES Order_(id) ON DELETE SET NULL,
    FOREIGN KEY(id_booking) REFERENCES Booking(id) ON DELETE CASCADE
);


CREATE TABLE MenuDish(
    id_menu INTEGER,
    id_dish INTEGER,
    PRIMARY KEY(id_menu, id_dish),  
    FOREIGN KEY(id_menu) REFERENCES Menu(id) ON DELETE CASCADE,
    FOREIGN KEY(id_dish) REFERENCES Dish(id) ON DELETE CASCADE
);


CREATE TABLE OrderDish(
    id SERIAL PRIMARY KEY,
    id_order INTEGER NOT NULL,
    id_dish INTEGER NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY(id_order) REFERENCES Order_(id) ON DELETE CASCADE,
    FOREIGN KEY(id_dish) REFERENCES Dish(id) ON DELETE CASCADE,
    CONSTRAINT unique_order_dish UNIQUE (id_order, id_dish)
);


CREATE TABLE Payment(
    id SERIAL PRIMARY KEY,
    id_order INTEGER NOT NULL,
    total DECIMAL(12, 2) NOT NULL,
    payment_method VARCHAR(30) NOT NULL DEFAULT 'Готівка',
    FOREIGN KEY(id_order) REFERENCES Order_(id) ON DELETE CASCADE
);