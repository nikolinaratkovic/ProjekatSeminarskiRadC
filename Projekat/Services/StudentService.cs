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

        public async Task<List<Student>> UcitajSveStudenteAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student?> PronadjiStudentaPoIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.Ispiti)
                .FirstOrDefaultAsync(s => s.StudentID == studentId);
        }

        public async Task<Student> DodajStudentaAsync(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            bool postoji = await _context.Students
                .AnyAsync(s => s.BrojIndeksa == student.BrojIndeksa);

            if (postoji)
                throw new InvalidOperationException($"Student sa brojem indeksa '{student.BrojIndeksa}' već postoji.");

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return student;
        }

        public async Task AzurirajStudentaAsync(int studentId, Student noviPodaci)
        {
            if (noviPodaci == null)
                throw new ArgumentNullException(nameof(noviPodaci));

            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
                throw new InvalidOperationException($"Student sa ID-om {studentId} nije pronađen.");

            if (student.BrojIndeksa != noviPodaci.BrojIndeksa)
            {
                bool postoji = await _context.Students
                    .AnyAsync(s => s.BrojIndeksa == noviPodaci.BrojIndeksa);

                if (postoji)
                    throw new InvalidOperationException($"Student sa brojem indeksa '{noviPodaci.BrojIndeksa}' već postoji.");
            }

            student.Ime = noviPodaci.Ime;
            student.Prezime = noviPodaci.Prezime;
            student.BrojIndeksa = noviPodaci.BrojIndeksa;
            student.GodinaStudija = noviPodaci.GodinaStudija;

            await _context.SaveChangesAsync();
        }

        public async Task ObrisiStudentaAsync(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Ispiti)
                .FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null)
                throw new InvalidOperationException($"Student sa ID-om {studentId} nije pronađen.");

            if (student.Ispiti.Any())
            {
                _context.Ispiti.RemoveRange(student.Ispiti);
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}