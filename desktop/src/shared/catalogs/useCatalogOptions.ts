import { useEffect, useState } from 'react';
import type { CrudOption } from '../crud/CrudTypes';
import type { PagedResult } from '../http/ApiResponse';
import { HttpClient } from '../http/HttpClient';

// Solo los campos que se muestran en las opciones; los tipos completos viven en cada módulo.
interface CodeNameRow { Id: number; Code: string; Name: string }
interface PlanRow { Id: number; Code: string; Version: number | string }
interface StudentRow { Id: number; StudentNumber: string; FullName: string }
interface GroupRow { Id: number; Code: string; EnrolledCount: number; Capacity: number }

export interface CatalogOptions {
  Careers: CrudOption[];
  Terms: CrudOption[];
  Plans: CrudOption[];
  Students: CrudOption[];
  Groups: CrudOption[];
}

// Se consulta la API directamente (no los servicios de modules/) para que shared/ no dependa de los módulos.
async function GetFirstPage<TRow>(Path: string): Promise<TRow[]> {
  const Page = await HttpClient.Get<PagedResult<TRow>>(Path, { Page: 1, PageSize: 100, Search: '' });
  return Page.Items;
}

// Cache de catálogos: se consulta una vez por sesión.
let Cache: Promise<CatalogOptions> | null = null;

async function LoadOptions(): Promise<CatalogOptions> {
  if (Cache === null) {
    Cache = (async () => {
      const [Careers, Terms, Plans, Students, Groups] = await Promise.all([
        GetFirstPage<CodeNameRow>('/api/v1/careers'),
        GetFirstPage<CodeNameRow>('/api/v1/academic-terms'),
        GetFirstPage<PlanRow>('/api/v1/study-plans'),
        GetFirstPage<StudentRow>('/api/v1/students'),
        GetFirstPage<GroupRow>('/api/v1/class-groups'),
      ]);

      return {
        Careers: Careers.map((Career) => ({ value: String(Career.Id), label: `${Career.Code} · ${Career.Name}` })),
        Terms: Terms.map((Term) => ({ value: String(Term.Id), label: `${Term.Code} · ${Term.Name}` })),
        Plans: Plans.map((Plan) => ({ value: String(Plan.Id), label: `${Plan.Code} v${Plan.Version}` })),
        Students: Students.map((Student) => ({
          value: String(Student.Id),
          label: `${Student.StudentNumber} · ${Student.FullName}`,
        })),
        Groups: Groups.map((Group) => ({
          value: String(Group.Id),
          label: `${Group.Code} · ${Group.EnrolledCount}/${Group.Capacity}`,
        })),
      };
    })();
  }

  return Cache;
}

// Opciones de catálogo (carreras, periodos, planes, alumnos y grupos) para selects y filtros.
export function useCatalogOptions(): CatalogOptions {
  const [Options, SetOptions] = useState<CatalogOptions>({
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
