using Microsoft.EntityFrameworkCore;
using Projekat.Data;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekat.Services
{
    public class StudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> ucitajSveStudente()
        {
            try
            {
                return await _context.Students.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ucitavanju studenata: {ex.Message}", ex);
            }
        }

        public async Task<Student> pronadjiStudentaPoID(int studentID)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Ispiti)
                    .FirstOrDefaultAsync(s => s.StudentID == studentID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri pronalazenju studenta: {ex.Message}", ex);
            }
        }

        public async Task<Student> DodajStudenta(Student student)
        {
            try
            {
                if (student == null)
                    throw new ArgumentNullException(nameof(student));

                var postojeciStudent = await _context.Students
                    .FirstOrDefaultAsync(s => s.BrojIndeksa == student.BrojIndeksa);

                if (postojeciStudent != null)
                    throw new InvalidOperationException($"Student sa brojem indeksa '{student.BrojIndeksa}' ve? postoji.");

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return student;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri dodavanju studenta u bazu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri dodavanju studenta: {ex.Message}", ex);
            }
        }

    
        public async Task<Student> AzurirajStudenta(int studentID, Student noviPodaci)
        {
            try
            {
                if (noviPodaci == null)
                    throw new ArgumentNullException(nameof(noviPodaci));

                var student = await _context.Students.FindAsync(studentID);

                if (student == null)
                    throw new InvalidOperationException($"Student sa ID-om {studentID} nije prona?en.");

         
                if (student.BrojIndeksa != noviPodaci.BrojIndeksa)
                {
                    var postojeciStudent = await _context.Students
                        .FirstOrDefaultAsync(s => s.BrojIndeksa == noviPodaci.BrojIndeksa);

                    if (postojeciStudent != null)
                        throw new InvalidOperationException($"Student sa brojem indeksa '{noviPodaci.BrojIndeksa}' ve? postoji.");
                }

     
                student.Ime = noviPodaci.Ime;
                student.Prezime = noviPodaci.Prezime;
                student.BrojIndeksa = noviPodaci.BrojIndeksa;
                student.GodinaStudija = noviPodaci.GodinaStudija;

                _context.Students.Update(student);
                await _context.SaveChangesAsync();

                return student;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri ažuriranju studenta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri ažuriranju studenta: {ex.Message}", ex);
            }
        }

      
        public async Task ObrisiStudenta(int studentID)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Ispiti)
                    .FirstOrDefaultAsync(s => s.StudentID == studentID);

                if (student == null)
                    throw new InvalidOperationException($"Student sa ID-om {studentID} nije prona?en.");

     
                if (student.Ispiti.Any())
                {
                    _context.Ispiti.RemoveRange(student.Ispiti);
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Greška pri brisanju studenta: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri brisanju studenta: {ex.Message}", ex);
            }
        }
    }
}
