
#import "RoarSKStore.h"

void _StoreKitInit(const char* unityGameObject);
bool _StoreKitPurchasesEnabled();
void _StoreKitRequestProductData(const char* productIdentifiers);
void _StoreKitPurchase(const char* productIdentifier);
void _StoreKitPurchaseQuantity(const char* productIdentifier, int quantity);
