import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    let currectUser: User;

    this.accountService.currentUserSource.pipe(take(1)).subscribe((res) => {
      currectUser = res;
    });

    if (currectUser) {
      request = request.clone({
        setHeaders: {
          Authorization: 'Bearer ' + currectUser.token,
        },
      });
    }

    return next.handle(request);
  }

  //! END
}
