import { Routes } from '@angular/router';

const Routing: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
  },
  {
    path: 'builder',
    loadChildren: () => import('./builder/builder.module').then((m) => m.BuilderModule),
  },
  {
    path: 'crafted/pages/profile',
    loadChildren: () => import('../modules/profile/profile.module').then((m) => m.ProfileModule),
    // data: { layout: 'light-sidebar' },
  },
  {
    path: 'crafted/account',
    loadChildren: () => import('../modules/account/account.module').then((m) => m.AccountModule),
    // data: { layout: 'dark-header' },
  },
  {
    path: 'crafted/pages/wizards',
    loadChildren: () => import('../modules/wizards/wizards.module').then((m) => m.WizardsModule),
    // data: { layout: 'light-header' },
  },
  {
    path: 'crafted/widgets',
    loadChildren: () => import('../modules/widgets-examples/widgets-examples.module').then((m) => m.WidgetsExamplesModule),
    // data: { layout: 'light-header' },
  },
  {
    path: 'apps/chat',
    loadChildren: () => import('../modules/apps/chat/chat.module').then((m) => m.ChatModule),
    // data: { layout: 'light-sidebar' },
  },
  {
    path: 'apps/users',
    loadChildren: () => import('./user/user.module').then((m) => m.UserModule),
  },
  {
    path: 'apps/roles',
    loadChildren: () => import('./role/role.module').then((m) => m.RoleModule),
  },
  {
    path: 'apps/permissions',
    loadChildren: () => import('./permission/permission.module').then((m) => m.PermissionModule),
  },
  //==============================================================================================
  {
    path: 'apps/tutorial',
    loadComponent: () => import('./tutorial/tutorial-table-list/tutorial-table-list.component').then(mod => mod.TutorialTableListComponent)
  },
  {
    path: 'bacdaotao',
    loadComponent: () => import('./danhmuc/bac-dao-tao/bac-dao-tao-list/bac-dao-tao-list.component').then(mod => mod.BacDaoTaoTableListComponent)
  },
  //  {
  //   path: 'namhoc',
  //   loadComponent: () => import('./namhoc/nam-hoc-list/nam-hoc-list.component').then(mod => mod.NamHocTableListComponent)
  // },
  {
    path: 'loaidaotao',
    loadComponent: () => import('./danhmuc/loai-dao-tao/loai-dao-tao-list/loai-dao-tao.component').then(mod => mod.LoaiDaoTaoTableListComponent)
  },
    {
    path: 'dot-tuyen-sinh',
    loadComponent: () => import('./dottuyensinh/dot-tuyen-sinh-list/dot-tuyen-sinh-list.component').then(mod => mod.DotTuyenSinhTableListComponent)
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'error/404',
  },
];

export { Routing };
