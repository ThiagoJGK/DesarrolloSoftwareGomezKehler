import type { EntityDto } from '@abp/ng.core';

export interface NotificationDto extends EntityDto<string> {
  userId?: string;
  title?: string;
  message?: string;
  isRead: boolean;
  creationTime?: string;
}
