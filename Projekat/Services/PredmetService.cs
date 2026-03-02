using Microsoft.EntityFrameworkCore;
using Projekat.Data;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekat.Services
{
    public class PredmetService
    {
        private readonly ApplicationDbContext _context;

        public PredmetService(ApplicationDbContext context)
        {
            _context = context;
        }

          public async Task<List<Predmet>> ucitajSvePredmete()
        {
            try
            {
                return await _context.Predmeti.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri u?itavanju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> prondjPredmetPoID(int predmetID)
        {
            try
            {
                return await _context.Predmeti
                    .Include(p => p.Ispiti)
                    .FirstOrDefaultAsync(p => p.PredmetID == predmetID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri pronalaženju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> dodajPredmet(Predmet predmet)
        {
            try
            {
                if (predmet == null)
                    throw new ArgumentNullException(nameof(predmet));

                var postojeciPredmet = await _context.Predmeti
                    .FirstOrDefaultAsync(p => p.Naziv == predmet.Naziv);

                if (postojeciPredmet != null)
                    throw new InvalidOperationException($"Predmet sa nazivom '{predmet.Naziv}' ve? postoji.");

                _context.Predmeti.Add(predmet);
                await _context.SaveChangesAsync();

                return predmet;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri dodavanju predmeta u bazu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri dodavanju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> azurirajPredmet(int predmetID, Predmet noviPodaci)
        {
            try
            {
                if (noviPodaci == null)
                    throw new ArgumentNullException(nameof(noviPodaci));

                var predmet = await _context.Predmeti.FindAsync(predmetID);

                if (predmet == null)
                    throw new InvalidOperationException($"Predmet sa ID-om {predmetID} nije prona?en.");

                if (predmet.Naziv != noviPodaci.Naziv)
                {
                    var postojeciPredmet = await _context.Predmeti
                        .FirstOrDefaultAsync(p => p.Naziv == noviPodaci.Naziv);

                    if (postojeciPredmet != null)
                        throw new InvalidOperationException($"Predmet sa nazivom '{noviPodaci.Naziv}' ve? postoji.");
                }

                predmet.Naziv = noviPodaci.Naziv;
                predmet.ESPB = noviPodaci.ESPB;
                predmet.Semestar = noviPodaci.Semestar;

                _context.Predmeti.Update(predmet);
                await _context.SaveChangesAsync();

                return predmet;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri ažuriranju predmeta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ažuriranju predmeta: {ex.Message}", ex);
            }
        }

        public async Task obrisiPredmet(int predmetID)
        {
            try
            {
                var predmet = await _context.Predmeti
                    .Include(p => p.Ispiti)
                    .FirstOrDefaultAsync(p => p.PredmetID == predmetID);

                if (predmet == null)
                    throw new InvalidOperationException($"Predmet sa ID-om {predmetID} nije prona?en.");

                if (predmet.Ispiti.Any())
                {
                    _context.Ispiti.RemoveRange(predmet.Ispiti);
                }

                _context.Predmeti.Remove(predmet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri brisanju predmeta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brisanju predmeta: {ex.Message}", ex);
            }
        }
    }
}
