import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  // model: {
  //   username: string;
  //   password: string;
  // };
  model: {};

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {

  }

  login(form: NgForm) {
    this.model = {
      username: form.value.username,
      password: form.value.password,
    };

    this.accountService.login(this.model).subscribe(
      (res) => {
        console.log(this.model);
        console.log(res);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  logout() {
    this.accountService.logout();
  }
}
