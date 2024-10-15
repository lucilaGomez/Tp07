import { Component, OnInit } from '@angular/core';
import { Employee } from '../employee.model';
import { EmployeeService } from '../employee.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-addemployee',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './addemployee.component.html',
  styleUrls: ['./addemployee.component.css']
})
export class AddemployeeComponent implements OnInit {
  newEmployee: Employee = new Employee(0, '', '');
  submitBtnText: string = "Create";
  imgLoadingDisplay: string = 'none';

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      const employeeId = params['id'];
      if (employeeId) {
        this.editEmployee(employeeId);
      }
    });
  }

  addEmployee(employee: Employee) {
    // Validación 1: El nombre no puede estar vacío o ser solo espacios
    if (!employee.name.trim()) {
      this.toastr.error('El nombre no puede estar vacío o compuesto solo de espacios.');
      return;
    }

    // Validación 2: El nombre y apellido deben tener una longitud máxima de 100 caracteres
    if (employee.name.length > 100) {
      this.toastr.error('El nombre y apellido deben tener una longitud máxima de 100 caracteres.');
      return;
    }

    // Validación 3: El nombre debe tener al menos dos caracteres
    if (employee.name.length < 2) {
      this.toastr.error('El nombre debe tener al menos dos caracteres.');
      return;
    }

    // Validación 4: El nombre no debe contener números
    if (/\d/.test(employee.name)) {
      this.toastr.error('El nombre no debe contener números.');
      return;
    }

    // Validación 5: Cada parte del nombre debe tener al menos dos caracteres
    const nameParts = employee.name.split(' ').filter(part => part.trim().length > 0);
    for (const part of nameParts) {
      if (part.length < 2) {
        this.toastr.error('Cada parte del nombre debe tener al menos dos caracteres.');
        return;
      }
    }

    if (employee.id === 0) {
      employee.createdDate = new Date().toISOString();
      this.employeeService.createEmployee(employee).subscribe(result => this.router.navigate(['/']));
    } else {
      employee.createdDate = new Date().toISOString();
      this.employeeService.updateEmployee(employee).subscribe(result => this.router.navigate(['/']));
    }

    this.submitBtnText = "";
    this.imgLoadingDisplay = 'inline';
  }

  editEmployee(employeeId: number) {
    this.employeeService.getEmployeeById(employeeId).subscribe(res => {
      this.newEmployee.id = res.id;
      this.newEmployee.name = res.name;
      this.submitBtnText = "Edit";
    });
  }
}
