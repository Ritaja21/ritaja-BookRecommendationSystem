import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
    HttpTestingController,
    provideHttpClientTesting
} from '@angular/common/http/testing';

import { AuthService } from './auth.services';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {

    let service: AuthService;
    let httpMock: HttpTestingController;

    beforeEach(() => {

        localStorage.clear();

        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });

        service = TestBed.inject(AuthService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
        localStorage.clear();
    });

    it('should call register API', () => {

        const registerData = {
            name: 'John',
            email: 'john@test.com',
            password: '123456'
        };

        service.register(registerData).subscribe();

        const req = httpMock.expectOne(
            `${environment.apiUrl}/auth/register`
        );

        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(registerData);
    });

    it('should call login API', () => {

        const loginData = {
            email: 'john@test.com',
            password: '123456'
        };

        service.login(loginData).subscribe();

        const req = httpMock.expectOne(
            `${environment.apiUrl}/auth/login`
        );

        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(loginData);
    });

    it('should save token and user in localStorage on login', () => {

        const loginData = {
            email: 'john@test.com',
            password: '123456'
        };

        const mockResponse = {
            data: {
                token: 'fake-jwt-token',
                userDTO: {
                    id: 1,
                    name: 'John',
                    email: 'john@test.com',
                    role: 'Customer'
                }
            }
        };

        service.login(loginData).subscribe();

        const req = httpMock.expectOne(
            `${environment.apiUrl}/auth/login`
        );

        req.flush(mockResponse);

        expect(localStorage.getItem('token'))
            .toBe('fake-jwt-token');

        expect(localStorage.getItem('user'))
            .toContain('John');
    });

    it('should logout user', () => {

        localStorage.setItem('token', 'abc');
        localStorage.setItem('user', '{}');

        service.logout();

        expect(localStorage.getItem('token')).toBeNull();
        expect(localStorage.getItem('user')).toBeNull();
    });

    it('should return token', () => {

        localStorage.setItem('token', 'abc123');

        expect(service.getToken())
            .toBe('abc123');
    });

    it('should return true if logged in', () => {

        localStorage.setItem('token', 'abc');

        expect(service.isLoggedIn())
            .toBeTrue();
    });

    it('should return false if not logged in', () => {

        expect(service.isLoggedIn())
            .toBeFalse();
    });

    it('should return current user', () => {

        const user = {
            id: 1,
            name: 'John',
            role: 'Customer'
        };

        localStorage.setItem(
            'user',
            JSON.stringify(user)
        );

        expect(service.getUser()?.name)
            .toBe('John');
    });

    it('should return user role', () => {

        const user = {
            id: 1,
            name: 'John',
            role: 'Admin'
        };

        localStorage.setItem(
            'user',
            JSON.stringify(user)
        );

        expect(service.getRole())
            .toBe('Admin');
    });

});