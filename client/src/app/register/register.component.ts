import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelFromRegisterComponent = new EventEmitter();
  model: any = {};

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  register(form: NgForm) {
    this.model = {
      username: form.value.username,
      password: form.value.password,
    };
    console.log(this.model);

    this.accountService.register(this.model).subscribe(
      (res) => {
        console.log(res);
        this.cancel();
      },
      (err) => {
        console.log(err);
        this.toastr.error(err.error);
      }
    );
  }

  cancel() {
    this.cancelFromRegisterComponent.emit(false);
  }
}
