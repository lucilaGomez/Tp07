import { TestBed } from '@angular/core/testing'; 
import { AddemployeeComponent } from './addemployee.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs'; // para simular observables
import { DatePipe } from '@angular/common';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { Employee } from '../employee.model';

describe('AddemployeeComponent', () => {
  let component: AddemployeeComponent;
  let fixture: any;
  let toastrService: ToastrService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AddemployeeComponent, HttpClientTestingModule, ToastrModule.forRoot()],
      providers: [
        DatePipe,
        {
          provide: ActivatedRoute, // Simula ActivatedRoute
          useValue: {
            params: of({ id: 1 }) // simula el parámetro id en la URL
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddemployeeComponent);
    component = fixture.componentInstance;
    toastrService = TestBed.inject(ToastrService);

    spyOn(toastrService, 'error'); // Simula la llamada a error de Toastr
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // Validación 1: El nombre no puede estar vacío o compuesto solo de espacios
  it('should show error if name is empty or only spaces', () => {
    const employee: Employee = new Employee(0, '   ', 'Doe'); // Nombre vacío o solo espacios
    component.addEmployee(employee);
    expect(toastrService.error).toHaveBeenCalledWith('El nombre no puede estar vacío o compuesto solo de espacios.');
  });

  // Validación 2: El nombre y apellido deben tener una longitud máxima de 100 caracteres
  it('should show error if name exceeds 100 characters', () => {
    const longName = 'A'.repeat(101);
    const employee: Employee = new Employee(0, longName, 'Doe');
    component.addEmployee(employee);
    expect(toastrService.error).toHaveBeenCalledWith('El nombre y apellido deben tener una longitud máxima de 100 caracteres.');
  });

  // Validación 3: El nombre debe tener al menos dos caracteres
  it('should show error if name is less than 2 characters', () => {
    const employee: Employee = new Employee(0, 'A', 'Doe');
    component.addEmployee(employee);
    expect(toastrService.error).toHaveBeenCalledWith('El nombre debe tener al menos dos caracteres.');
  });

  // Validación 4: El nombre no debe contener números
  it('should show error if name contains numbers', () => {
    const employee: Employee = new Employee(0, 'John123', 'Doe');
    component.addEmployee(employee);
    expect(toastrService.error).toHaveBeenCalledWith('El nombre no debe contener números.');
  });

  // Validación 5: Cada parte del nombre debe tener al menos dos caracteres
  it('should show error if any part of the name is less than 2 characters', () => {
    const employee: Employee = new Employee(0, 'J A', 'Doe'); // Partes del nombre con menos de 2 caracteres
    component.addEmployee(employee);
    expect(toastrService.error).toHaveBeenCalledWith('Cada parte del nombre debe tener al menos dos caracteres.');
  });
});
