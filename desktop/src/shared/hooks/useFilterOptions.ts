import { useEffect, useState } from 'react';
import { CareersService } from '../../modules/Careers/model/CareersService';
import { AcademicTermsService } from '../../modules/AcademicTerms/model/AcademicTermsService';
import { StudyPlansService } from '../../modules/StudyPlans/model/StudyPlansService';
import { StudentsService } from '../../modules/Students/model/StudentsService';
import { ClassGroupsService } from '../../modules/ClassGroups/model/ClassGroupsService';
import type { CrudOption } from '../crud/CrudTypes';

export type FilterOptionKey = 'Careers' | 'Terms' | 'Plans' | 'ActiveStatus' | 'StudentStatuses';

interface FilterOptions {
  Careers: CrudOption[];
  Terms: CrudOption[];
  Plans: CrudOption[];
  Students: CrudOption[];
  Groups: CrudOption[];
}

// Cache de catálogos para los filtros: se consulta una vez por sesión.
let Cache: Promise<FilterOptions> | null = null;

async function LoadOptions(): Promise<FilterOptions> {
  if (Cache === null) {
    Cache = (async () => {
      const [CareerPage, TermPage, PlanPage, StudentPage, GroupPage] = await Promise.all([
        CareersService.GetPaged(1, 100, ''),
        AcademicTermsService.GetPaged(1, 100, ''),
        StudyPlansService.GetPaged(1, 100, ''),
        StudentsService.GetPaged(1, 100, ''),
        ClassGroupsService.GetPaged(1, 100, ''),
      ]);

      return {
        Careers: CareerPage.Items.map((Career) => ({
          value: String(Career.Id),
          label: `${Career.Code} · ${Career.Name}`,
        })),
        Terms: TermPage.Items.map((Term) => ({
          value: String(Term.Id),
          label: `${Term.Code} · ${Term.Name}`,
        })),
        Plans: PlanPage.Items.map((Plan) => ({
          value: String(Plan.Id),
          label: `${Plan.Code} v${Plan.Version}`,
        })),
        Students: StudentPage.Items.map((Student) => ({
          value: String(Student.Id),
          label: `${Student.StudentNumber} · ${Student.FullName}`,
        })),
        Groups: GroupPage.Items.map((Group) => ({
          value: String(Group.Id),
          label: `${Group.Code} · ${Group.EnrolledCount}/${Group.Capacity}`,
        })),
      };
    })();
  }

  return Cache;
}

// Opciones de catálogo para los filtros de tablas.
export function useFilterOptions(): FilterOptions {
  const [Options, SetOptions] = useState<FilterOptions>({
    Careers: [],
    Terms: [],
    Plans: [],
    Students: [],
    Groups: [],
  });

  useEffect(() => {
    let Active = true;

    LoadOptions()
      .then((Loaded) => {
        if (Active) {
          SetOptions(Loaded);
        }
      })
      .catch(() => {
        // Sin catálogos los filtros quedan vacíos; la tabla sigue funcionando
      });

    return () => {
      Active = false;
    };
  }, []);

  return Options;
}
