import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // httpOptions = {
  //   headers: new HttpHeaders({
  //     Authorization:
  //       'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token,
  //   }),
  // };

  // paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUserSource.pipe(take(1)).subscribe( user => {
      this.user = user;
      this.userParams = new UserParams(user)
    }) 
  }

  getUserParams() {
    return this.userParams
  }
  setUserParams(userParms: UserParams) {
    this.userParams = userParms
  }
  resetUserParams() {
    this.userParams = new UserParams(this.user)
    return this.userParams
  }

  getMembers(userParams: UserParams) {
    // if (this.members.length > 0) return of(this.members);
    // console.log(Object.values(userParams).join('-'));
    const paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();


    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //! return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
    //   map((res) => {
    //     this.memberCache.set(Object.values(userParams).join('-'), res);
    //     return res;
    //   })
    //! );

    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }),
      map(res => {
        this.memberCache.set(Object.values(userParams).join('-'), res);
        return res;
      })
    );

  }

  //! private getPaginatedResult<T>(url, params) {
  //   const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  //   return this.http.get<T>(url, { observe: 'response', params }).pipe(
  //     map((response) => {
  //       paginatedResult.result = response.body;
  //       if (response.headers.get('Pagination') !== null) {
  //         paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
  //       }
  //       return paginatedResult;
  //     })
  //   );
  //! }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    //check if there are any params and if yes set them to the GET request
    // if (page !== null && itemsPerPage !== null) {
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    // }
    return params;
  }

  getMember(username: string) {
    // const member = this.members.find((x) => {
    //   x.username === username;
    // });
    // if (member !== undefined) return of(member);

    //Cache check
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find(member => member.username === username)


    if (member) {
      return of(member)
    }
    //--

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put<Member>(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string, pageNumber, pageSize) {
    const paginatedResult: PaginatedResult<Partial<Member[]>> = new PaginatedResult<Partial<Member[]>>();

    let params = this.getPaginationHeaders(pageNumber,pageSize);

    params = params.append('predicate', predicate);

    return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes', { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }))
    
  }

  //! END
}
