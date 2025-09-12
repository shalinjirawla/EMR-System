import { Component, ChangeDetectionStrategy, OnInit, Injector } from '@angular/core';
import { filter as _filter } from 'lodash-es';
import { BsDropdownDirective, BsDropdownToggleDirective, BsDropdownMenuDirective } from 'ngx-bootstrap/dropdown';
import { MenuModule } from 'primeng/menu';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { UserServiceProxy, ChangeUserLanguageDto } from '../../shared/service-proxies/service-proxies';
import { AppComponentBase } from '../../shared/app-component-base';
import { BadgeModule } from 'primeng/badge';
import { OverlayBadgeModule } from 'primeng/overlaybadge';

@Component({
  selector: 'app-header-notification-menu',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  templateUrl: './header-notification-menu.component.html',
  styleUrl: './header-notification-menu.component.css',
  imports: [AvatarModule, AvatarGroupModule, BadgeModule, OverlayBadgeModule, MenuModule],
})
export class HeaderNotificationMenuComponent extends AppComponentBase implements OnInit {
 

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
  }
}