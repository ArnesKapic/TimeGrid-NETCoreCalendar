using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreCalendar.Data
{
    public interface IDAL
    {
        List<Event> GetEvents();
        List<Event> GetMyEvents(string userid);
        Event GetEvent(int id);
        void CreateEvent(IFormCollection form);
        void UpdateEvent(IFormCollection form);
        void DeleteEvent(int id);

        List<Location> GetLocations();
        Location GetLocation(int id);
        void CreateLocation(Location location);

        // NEW:
        void UpdateLocation(Location location);
        void DeleteLocation(int id);
    }

    public class DAL : IDAL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Event> GetEvents()
        {
            return db.Events.ToList();
        }

        public List<Event> GetMyEvents(string userid)
        {
            return db.Events.Where(x => x.User.Id == userid).ToList();
        }

        public Event GetEvent(int id)
        {
            return db.Events.FirstOrDefault(x => x.Id == id);
        }

        public void CreateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            var newevent = new Event(form, db.Locations.FirstOrDefault(x => x.Name == locname), user);
            db.Events.Add(newevent);
            db.SaveChanges();
        }

        public void UpdateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var eventid = int.Parse(form["Event.Id"]);
            var myevent = db.Events.FirstOrDefault(x => x.Id == eventid);
            var location = db.Locations.FirstOrDefault(x => x.Name == locname);
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            myevent.UpdateEvent(form, location, user);
            db.Entry(myevent).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteEvent(int id)
        {
            var myevent = db.Events.Find(id);
            db.Events.Remove(myevent);
            db.SaveChanges();
        }

        public List<Location> GetLocations()
        {
            return db.Locations.ToList();
        }

        public Location GetLocation(int id)
        {
            return db.Locations.Find(id);
        }

        public void CreateLocation(Location location)
        {
            db.Locations.Add(location);
            db.SaveChanges();
        }

        // ===== NEW: EDIT LOCATION =====
        public void UpdateLocation(Location location)
        {
            var existing = db.Locations.Find(location.Id);
            if (existing == null) return;
            existing.Name = location.Name;
            db.Entry(existing).State = EntityState.Modified;
            db.SaveChanges();
        }

        // ===== NEW: DELETE LOCATION (handles related events without LocationId property) =====
        public void DeleteLocation(int id)
        {
            // Events reference Location via a shadow FK "LocationId" since Event has no LocationId property.
            var relatedEvents = db.Events
                .Where(e => EF.Property<int?>(e, "LocationId") == id)
                .ToList();

            foreach (var ev in relatedEvents)
            {
                ev.Location = null;
                db.Entry(ev).Property("LocationId").CurrentValue = null;
                db.Entry(ev).State = EntityState.Modified;
            }

            var loc = db.Locations.Find(id);
            if (loc == null) return;

            db.Locations.Remove(loc);
            db.SaveChanges();
        }
    }
}


