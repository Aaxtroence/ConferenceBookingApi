using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Rooms.Any())
                return;

            var roomA = new Room { Name = "Зал А", Capacity = 50, BasePricePerHour = 2000 };
            var roomB = new Room { Name = "Зал Б", Capacity = 100, BasePricePerHour = 3500 };
            var roomC = new Room { Name = "Зал В", Capacity = 30, BasePricePerHour = 1500 };

            context.Rooms.AddRange(roomA, roomB, roomC);
            context.SaveChanges();

            context.Equipment.AddRange(
                new Equipment { Name = "Проєктор", Price = 500, RoomId = roomA.Id },
                new Equipment { Name = "Wi-Fi", Price = 300, RoomId = roomA.Id },
                new Equipment { Name = "Звук", Price = 700, RoomId = roomA.Id },

                new Equipment { Name = "Проєктор", Price = 500, RoomId = roomB.Id },
                new Equipment { Name = "Wi-Fi", Price = 300, RoomId = roomB.Id },
                new Equipment { Name = "Звук", Price = 700, RoomId = roomB.Id },

                new Equipment { Name = "Проєктор", Price = 500, RoomId = roomC.Id },
                new Equipment { Name = "Wi-Fi", Price = 300, RoomId = roomC.Id }
            );

            context.SaveChanges();
        }
    }
}