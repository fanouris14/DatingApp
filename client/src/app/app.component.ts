import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';


//| interface
/* interface IUser {
  id: number;
  userName: string;
} */
//| End

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'Our Dating App';
  users: /* IUser[]; */ any;


  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.http.get<any>('https://localhost:5001/api/users').subscribe(
      (res) => {
        this.users = res;
        console.log(this.users);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  //! END OF CLASS
}
