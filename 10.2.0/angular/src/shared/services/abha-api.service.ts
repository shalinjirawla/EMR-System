import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

@Injectable({
    providedIn: 'root'
})
export class AbhaApiService {
    protected baseUrl = AppConsts.remoteServiceBaseUrl;

    constructor(protected http: HttpClient) { }

    protected getAbhaHeaders(): HttpHeaders {
        return new HttpHeaders({
            'Content-Type': 'application/json'
        });
    }

    requestAadhaarOtp(aadhaarNumber: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaEnrollment/RequestAadhaarOtp`, { aadhaarNumber }, { headers: this.getAbhaHeaders() });
    }

    verifyAadhaarOtp(otp: string, txnId: string, mobileNumber: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaEnrollment/VerifyAadhaarOtp`, { otp, txnId, mobileNumber }, { headers: this.getAbhaHeaders() });
    }

    suggestAddresses(txnId: string): import("rxjs").Observable<any> {
        return this.http.get(`${this.baseUrl}/api/services/app/AbhaEnrollment/GetAddressSuggestions?txnId=${txnId}`, { headers: this.getAbhaHeaders() });
    }

    createAbhaAddress(patientId: number, abhaAddress: string, txnId: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaEnrollment/CreateAbhaAddress`, { patientId, abhaAddress, txnId }, { headers: this.getAbhaHeaders() });
    }

    requestLoginOtp(loginId: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaLogin/RequestLoginOtp`, { loginId }, { headers: this.getAbhaHeaders() });
    }

    verifyLoginOtp(otp: string, txnId: string, loginId: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaLogin/VerifyLoginOtp`, { otp, txnId, loginId }, { headers: this.getAbhaHeaders() });
    }

    fetchAndLinkProfile(patientId: number, xToken: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaLogin/FetchAndLinkProfile`, { patientId, xToken }, { headers: this.getAbhaHeaders() });
    }

    getAbhaCard(xToken: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaLogin/GetAbhaCard`, { xToken }, { headers: this.getAbhaHeaders() });
    }

    getAbhaQrCode(xToken: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbhaLogin/GetAbhaQrCode`, { xToken }, { headers: this.getAbhaHeaders() });
    }

    initiateConsentRequest(patientId: number, doctorName: string, purposeCode: string = "CAREMGT"): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbdmConsent/InitiateConsentRequest`, { patientId, doctorName, purposeCode }, { headers: this.getAbhaHeaders() });
    }

    requestExternalHealthInformation(consentId: string): import("rxjs").Observable<any> {
        return this.http.post(`${this.baseUrl}/api/services/app/AbdmHealthInformation/RequestExternalHealthInformation`, { consentId }, { headers: this.getAbhaHeaders() });
    }

    getExternalHealthRecords(patientId: number): import("rxjs").Observable<any> {
        return this.http.get(`${this.baseUrl}/api/services/app/AbdmHealthInformation/GetExternalHealthRecords?patientId=${patientId}`, { headers: this.getAbhaHeaders() });
    }
}
