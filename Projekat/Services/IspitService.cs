using Microsoft.EntityFrameworkCore;
using Projekat.Data;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekat.DTOs;

namespace Projekat.Services
{
    public class IspitService
    {
        private readonly ApplicationDbContext _context;

        public IspitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ispit>> ucitajSveIspite()
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

        public async Task<Ispit> pronadjIspitPoID(int ispitID)
        {
            try
            {
                return await _context.Ispiti
                    .Include(i => i.Student)
                    .Include(i => i.Predmet)
                    .FirstOrDefaultAsync(i => i.IspitID == ispitID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri pronalaženju ispita: {ex.Message}", ex);
            }
        }

        public async Task<Ispit> dodajIspit(Ispit ispit)
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
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri dodavanju ispita u bazu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri dodavanju ispita: {ex.Message}", ex);
            }
        }

        public async Task<Ispit> azurirajIspit(int ispitID, Ispit noviPodaci)
        {
            try
            {
                if (noviPodaci == null)
                    throw new ArgumentNullException(nameof(noviPodaci));

                var ispit = await _context.Ispiti.FindAsync(ispitID);

                if (ispit == null)
                    throw new InvalidOperationException($"Ispit sa ID-om {ispitID} nije pronađen.");

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
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri ažuriranju ispita: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ažuriranju ispita: {ex.Message}", ex);
            }
        }

        public async Task obrisiIspit(int ispitID)
        {
            try
            {
                var ispit = await _context.Ispiti.FindAsync(ispitID);

                if (ispit == null)
                    throw new InvalidOperationException($"Ispit sa ID-om {ispitID} nije pronađen.");

                _context.Ispiti.Remove(ispit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri brisanju ispita: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brisanju ispita: {ex.Message}", ex);
            }
        }

        public async Task<int> brojPolozenihIspitaStudenta(int studentID)
        {
            try
            {
                return await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena >= 6)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brojanju položenih ispita: {ex.Message}", ex);
            }
        }

        public async Task<int> brojNepolozenihIspitaStudenta(int studentID)
        {
            try
            {
                return await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena < 6)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brojanju nepoloženih ispita: {ex.Message}", ex);
            }
        }

        public async Task<double> procenatPolozenihIspitaStudenta(int studentID)
        {
            try
            {
                var ukupno = await _context.Ispiti
                    .Where(i => i.StudentID == studentID)
                    .CountAsync();

                if (ukupno == 0)
                    return 0;

                var polozeni = await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena >= 6)
                    .CountAsync();

                return (double)polozeni / ukupno * 100;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri izračunavanju procenta položenih ispita: {ex.Message}", ex);
            }
        }

        public async Task<double> procenatNepolozenihIspitaStudenta(int studentID)
        {
            try
            {
                var ukupno = await _context.Ispiti
                    .Where(i => i.StudentID == studentID)
                    .CountAsync();

                if (ukupno == 0)
                    return 0;

                var nepolozeni = await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena < 6)
                    .CountAsync();

                return (double)nepolozeni / ukupno * 100;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri izračunavanju procenta nepoloženih ispita: {ex.Message}", ex);
            }
        }

        public async Task<double> izracunajProsecnuOcenuPredmeta(int predmetID)
        {
            try
            {
                var ocene = await _context.Ispiti
                    .Where(i => i.PredmetID == predmetID)
                    .Select(i => i.Ocena)
                    .ToListAsync();

                if (ocene.Count == 0)
                    return 0;

                return ocene.Average();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri izračunavanju prosečne ocene za predmet: {ex.Message}", ex);
            }
        }

        public async Task<double> izracunajProsecnuOcenuStudenta(int studentID)
        {
            try
            {
                var ocene = await _context.Ispiti
                    .Where(i => i.StudentID == studentID)
                    .Select(i => i.Ocena)
                    .ToListAsync();

                if (ocene.Count == 0)
                    return 0;

                return ocene.Average();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri izračunavanju prosečne ocene za studenta: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, int>> brojIspitaPoIspitimRokovimaSviStudenti()
        {
            try
            {
                var result = await _context.Ispiti
                    .GroupBy(i => i.IspitniRok)
                    .Select(g => new { IspitniRok = g.Key, Broj = g.Count() })
                    .AsNoTracking()
                    .ToListAsync();

                return result.ToDictionary(x => x.IspitniRok, x => x.Broj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brojanju ispita po ispitnim rokovima: {ex.Message}", ex);
            }
        }
    

    public async Task<List<SemestarStatistikaDTO>> getTrendUspehaBySemestar(int studentID)
        {
            try
            {
                return await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena >= 6)
                    .Include(i => i.Predmet)
                    .GroupBy(i => i.Predmet.Semestar)
                    .Select(g => new SemestarStatistikaDTO
                    {
                        Semester = g.Key,
                        ProsecnaOcena = g.Average(i => i.Ocena)
                    })
                    .OrderBy(s => s.Semester)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri učitavanju trenda uspeha: {ex.Message}", ex);
            }
        }

        public async Task<List<PeriodStatistikaDTO>> getUporedniPrikaz(
            int studentID,
            DateTime od1, DateTime do1,
            DateTime od2, DateTime do2)
        {
            try
            {
                var prosek1 = await _context.Ispiti
                    .Where(i => i.StudentID == studentID
                            && i.Ocena >= 6
                            && i.DatumPolaganja >= od1
                            && i.DatumPolaganja <= do1)
                    .AverageAsync(i => (double?)i.Ocena) ?? 0;

                var prosek2 = await _context.Ispiti
                    .Where(i => i.StudentID == studentID
                            && i.Ocena >= 6
                            && i.DatumPolaganja >= od2
                            && i.DatumPolaganja <= do2)
                    .AverageAsync(i => (double?)i.Ocena) ?? 0;

                return new List<PeriodStatistikaDTO>
        {
            new PeriodStatistikaDTO { Oznaka = $"{od1:MM/yyyy} - {do1:MM/yyyy}", ProsecnaOcena = prosek1 },
            new PeriodStatistikaDTO { Oznaka = $"{od2:MM/yyyy} - {do2:MM/yyyy}", ProsecnaOcena = prosek2 }
        };
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri učitavanju uporednog prikaza: {ex.Message}", ex);
            }
        }

        public async Task<List<VremeStatistikaDTO>> getKretanjeProsecneOcene(int studentID)
        {
            try
            {
                return await _context.Ispiti
                    .Where(i => i.StudentID == studentID && i.Ocena >= 6)
                    .GroupBy(i => new { i.DatumPolaganja.Year, i.DatumPolaganja.Month })
                    .Select(g => new VremeStatistikaDTO
                    {
                        Godina = g.Key.Year,
                        Mesec = g.Key.Month,
                        ProsecnaOcena = g.Average(i => i.Ocena)
                    })
                    .OrderBy(v => v.Godina)
                    .ThenBy(v => v.Mesec)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri učitavanju kretanja prosečne ocene: {ex.Message}", ex);
            }
        }
    }

    }