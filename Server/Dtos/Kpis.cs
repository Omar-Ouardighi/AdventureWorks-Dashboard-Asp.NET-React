namespace Server.Dtos;



public record class KpisDto
(
     decimal TotalSales,
     int TotalOrderQuantity,
     decimal AverageMoneySpent 
);