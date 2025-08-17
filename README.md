# ZeonParser
***ZeonParser*** - это парсер категорий и товаров интернет-магазина *[Зеон](https://zeon18.ru/)*. Он был написан на C# с использованием фреймворков *ASP.NET Core*, *Entity Framework Core* и *AngleSharp*. В качестве системы управления базами данных была использована *PostgreSQL*. 
## Установка
## Возможности
+ Парсинг категорий с сохранением иерархии (родительские/дочерние);
+ Сбор данных о товарах: названия, цен, описания, изображения;
+ Сохранение данных в PostgreSQL с использованием Entity Framework Core;
+ Получение данных из БД по HTTP-запросам.
## База данных
### Концептуальная модель данных
![Концептуальная модель данных](https://github.com/Kuro-D-Shiro/ZeonParser/blob/develop/Diagrams/%D0%9A%D0%BE%D0%BD%D1%86%D0%B5%D0%BF%D1%82%D1%83%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F%20%D0%BC%D0%BE%D0%B4%D0%B5%D0%BB%D1%8C%20%D0%91%D0%94%20Zeon%20Parser.png)
### Физическая модель данных 
![Физическая модель данных](https://github.com/Kuro-D-Shiro/ZeonParser/blob/develop/Diagrams/%D0%A4%D0%B8%D0%B7%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B0%D1%8F%20%D0%BC%D0%BE%D0%B4%D0%B5%D0%BB%D1%8C%20%D0%91%D0%94%20Zeon%20Parser.png)
