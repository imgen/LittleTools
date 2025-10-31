import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '/services/theme_service.dart';
import '/base/base_appbar_view.dart';
import '{page_name}_controller.dart';

class {PageName}Page extends BaseAppBarView<{PageName}Controller> {
  final themeService = Get.find<ThemeService>();

  {PageName}Page({super.key});

  @override
  Widget body(BuildContext context) {
    return Text("{PageTitle}");
  }

  @override
  String get title => "{PageTitle}";
}
