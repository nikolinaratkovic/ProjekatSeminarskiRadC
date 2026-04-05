using Microsoft.EntityFrameworkCore;
using Projekat.Data;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekat.Services
{
    public class IspitService
    {
        private readonly ApplicationDbContext _context;

        public IspitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ispit>> UcitajSveIspite()
        {
            try
            {
                return await _context.Ispiti
                    .Include(i => i.Student)
                    .Include(i => i.Predmet)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri učitavanju ispita: {ex.Message}", ex);
            }
        }

        public async Task<Ispit> PronadjiIspitPoId(int ispitId)
        {
            try
            {
                return await _context.Ispiti
                    .Include(i => i.Student)
                    .Include(i => i.Predmet)
                    .FirstOrDefaultAsync(i => i.IspitID == ispitId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri pronalaženju ispita: {ex.Message}", ex);
            }
        }

        public async Task<Ispit> DodajIspit(Ispit ispit)
        {
            try
            {
                if (ispit == null)
                    throw new ArgumentNullException(nameof(ispit));

                var studentPostoji = await _context.Students.AnyAsync(s => s.StudentID == ispit.StudentID);
                if (!studentPostoji)
                    throw new InvalidOperationException($"Student sa ID-om {ispit.StudentID} nije pronađen.");

                var predmetPostoji = await _context.Predmeti.AnyAsync(p => p.PredmetID == ispit.PredmetID);
                if (!predmetPostoji)
                    throw new InvalidOperationException($"Predmet sa ID-om {ispit.PredmetID} nije pronađen.");

                _context.Ispiti.Add(ispit);
                await _context.SaveChangesAsync();

                return ispit;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri dodavanju ispita: {ex.Message}", ex);
            }
        }

        public async Task<Ispit> AzurirajIspit(int ispitId, Ispit noviPodaci)
        {
            try
            {
                if (noviPodaci == null)
                    throw new ArgumentNullException(nameof(noviPodaci));

                var ispit = await _context.Ispiti.FindAsync(ispitId);
                if (ispit == null)
                    throw new InvalidOperationException($"Ispit sa ID-om {ispitId} nije pronađen.");

                var studentPostoji = await _context.Students.AnyAsync(s => s.StudentID == noviPodaci.StudentID);
                if (!studentPostoji)
                    throw new InvalidOperationException($"Student sa ID-om {noviPodaci.StudentID} nije pronađen.");

                var predmetPostoji = await _context.Predmeti.AnyAsync(p => p.PredmetID == noviPodaci.PredmetID);
                if (!predmetPostoji)
                    throw new InvalidOperationException($"Predmet sa ID-om {noviPodaci.PredmetID} nije pronađen.");

                ispit.StudentID = noviPodaci.StudentID;
                ispit.PredmetID = noviPodaci.PredmetID;
                ispit.Ocena = noviPodaci.Ocena;
                ispit.DatumPolaganja = noviPodaci.DatumPolaganja;
                ispit.IspitniRok = noviPodaci.IspitniRok;

                _context.Ispiti.Update(ispit);
                await _context.SaveChangesAsync();

                return ispit;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ažuriranju ispita: {ex.Message}", ex);
            }
        }

        public async Task ObrisiIspit(int ispitId)
        {
            try
            {
                var ispit = await _context.Ispiti.FindAsync(ispitId);
                if (ispit == null)
                    throw new InvalidOperationException($"Ispit sa ID-om {ispitId} nije pronađen.");

                _context.Ispiti.Remove(ispit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brisanju ispita: {ex.Message}", ex);
            }
        }

        public async Task<double> IzracunajProsecnuOcenuPredmeta(int predmetId)
        {
            var ocene = await _context.Ispiti
                .Where(i => i.PredmetID == predmetId)
                .Select(i => (double)i.Ocena)
                .ToListAsync();

            return ocene.Count == 0 ? 0 : Math.Round(ocene.Average(), 2);
        }

        public async Task<double> IzracunajProsecnuOcenuStudenta(int studentId)
        {
            var ocene = await _context.Ispiti
                .Where(i => i.StudentID == studentId)
                .Select(i => (double)i.Ocena)
                .ToListAsync();

            return ocene.Count == 0 ? 0 : Math.Round(ocene.Average(), 2);
        }

        public async Task<Dictionary<string, int>> BrojIspitaPoIspitimRokovimaSviStudenti()
        {
            var lista = await _context.Ispiti
                .GroupBy(i => i.IspitniRok)
                .Select(g => new { Rok = g.Key, Broj = g.Count() })
                .ToListAsync();

            return lista.ToDictionary(x => x.Rok, x => x.Broj);
        }
    }
}