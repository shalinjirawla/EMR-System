import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [LocalizePipe],
})
export class FooterComponent extends AppComponentBase {
    currentYear: number;
    versionText: string;

    constructor(injector: Injector) {
        super(injector);

        this.currentYear = new Date().getFullYear();
        this.versionText =
            this.appSession.application.version +
            ' [' +
            this.appSession.application.releaseDate.format('DD-MM-YYYY') +
            ']';
    }
}
