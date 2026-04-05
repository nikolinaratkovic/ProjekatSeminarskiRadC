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

        public async Task<List<Predmet>> UcitajSvePredmete()
        {
            try
            {
                return await _context.Predmeti
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri učitavanju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> PronadjiPredmetPoId(int predmetId)
        {
            try
            {
                return await _context.Predmeti
                    .Include(p => p.Ispiti)
                    .FirstOrDefaultAsync(p => p.PredmetID == predmetId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri pronalaženju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> DodajPredmet(Predmet predmet)
        {
            try
            {
                if (predmet == null)
                    throw new ArgumentNullException(nameof(predmet));

                var postojeciPredmet = await _context.Predmeti
                    .FirstOrDefaultAsync(p => p.Naziv.ToLower() == predmet.Naziv.ToLower());

                if (postojeciPredmet != null)
                    throw new InvalidOperationException($"Predmet sa nazivom '{predmet.Naziv}' već postoji.");

                _context.Predmeti.Add(predmet);
                await _context.SaveChangesAsync();

                return predmet;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri dodavanju predmeta: {ex.Message}", ex);
            }
        }

        public async Task<Predmet> AzurirajPredmet(int predmetId, Predmet noviPodaci)
        {
            try
            {
                if (noviPodaci == null)
                    throw new ArgumentNullException(nameof(noviPodaci));

                var predmet = await _context.Predmeti.FindAsync(predmetId);

                if (predmet == null)
                    throw new InvalidOperationException($"Predmet sa ID-om {predmetId} nije pronađen.");

                if (predmet.Naziv.ToLower() != noviPodaci.Naziv.ToLower())
                {
                    var postojeciPredmet = await _context.Predmeti
                        .FirstOrDefaultAsync(p => p.Naziv.ToLower() == noviPodaci.Naziv.ToLower());

                    if (postojeciPredmet != null)
                        throw new InvalidOperationException($"Predmet sa nazivom '{noviPodaci.Naziv}' već postoji.");
                }

                predmet.Naziv = noviPodaci.Naziv;
                predmet.ESPB = noviPodaci.ESPB;
                predmet.Semestar = noviPodaci.Semestar;

                _context.Predmeti.Update(predmet);
                await _context.SaveChangesAsync();

                return predmet;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ažuriranju predmeta: {ex.Message}", ex);
            }
        }

        public async Task ObrisiPredmet(int predmetId)
        {
            try
            {
                var predmet = await _context.Predmeti
                    .Include(p => p.Ispiti)
                    .FirstOrDefaultAsync(p => p.PredmetID == predmetId);

                if (predmet == null)
                    throw new InvalidOperationException($"Predmet sa ID-om {predmetId} nije pronađen.");

                _context.Ispiti.RemoveRange(predmet.Ispiti);

                _context.Predmeti.Remove(predmet);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brisanju predmeta: {ex.Message}", ex);
            }
        }
    }
}