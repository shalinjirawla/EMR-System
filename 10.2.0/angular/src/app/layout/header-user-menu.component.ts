import { Component, ChangeDetectionStrategy, OnInit, Injector } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { BsDropdownDirective, BsDropdownToggleDirective, BsDropdownMenuDirective } from 'ngx-bootstrap/dropdown';
import { RouterLink, Router } from '@angular/router';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { MenubarModule } from 'primeng/menubar';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { BadgeModule } from 'primeng/badge';
import { AppComponentBase } from '@shared/app-component-base';
import { ButtonModule } from 'primeng/button';
@Component({
    selector: 'header-user-menu',
    templateUrl: './header-user-menu.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    styleUrl: './header-user-menu.component.css',
    imports: [ButtonModule,BsDropdownDirective, MenuModule, OverlayPanelModule, BadgeModule, MenubarModule, AvatarGroupModule, AvatarModule, OverlayBadgeModule, BsDropdownToggleDirective, BsDropdownMenuDirective, RouterLink, LocalizePipe],
})
export class HeaderUserMenuComponent extends AppComponentBase implements OnInit {
    shownLoginName = '';
    constructor(private _authService: AppAuthService, injector: Injector) {
        super(injector);
    }
    logout(): void {
        this._authService.logout();
    }

    ngOnInit() {
        this.shownLoginName = this.appSession.getShownLoginName();
    }
}
