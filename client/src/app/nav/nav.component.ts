import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
  user: User;

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.getLoggedInUsername();
  }

  login(form: NgForm) {
    this.model = {
      username: form.value.username,
      password: form.value.password,
    };

    this.accountService.login(this.model).subscribe(
      (res) => {
        console.warn(this.model);
        console.warn(res);
        this.router.navigate(['/members']);
      },
      (err) => {
        console.log(err);
        // this.toastr.error(err.error);
      }
    );
  }

  getLoggedInUsername() {
    this.accountService.currentUserSource.subscribe((u) => {
      if (u) {
        this.user = u;
      }
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigate(['/']);
  }
}
