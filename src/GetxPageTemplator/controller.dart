import '{page_name}_provider.dart';
import '/base/base_controller.dart';

class {PageName}Controller extends BaseController {
  final {PageName}Provider _provider;

  {PageName}Controller(this._provider) : super(_provider);

  @override
  void onInit() {
    super.onInit();

    logger.d("{PageName} page is initializing");
  }
}
