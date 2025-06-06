import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    templateUrl: './about.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [LocalizePipe],
})
export class AboutComponent extends AppComponentBase {
    constructor(injector: Injector) {
        super(injector);
    }
}
