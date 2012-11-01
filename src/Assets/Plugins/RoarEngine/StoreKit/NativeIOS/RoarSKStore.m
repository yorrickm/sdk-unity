
#import "RoarSKStore.h"
#import <StoreKit/SKProductsRequest.h>

// RoarSKStore will call the following functions on the listener unity game object
// to notify the unity application of storekit activity:
//
// gameObject.OnAppstoreProductData(productDataXml)
// called when: products have been retrieved from the appstore
//
// gameObject.OnAppstoreRequestProductDataInvalidProductId(invalidProductId)
// called when: invalid product identifier specified when requesting product data,
//              called once for each invalid product identifier.
//
// gameObject.OnAppstoreProductPurchaseComplete(purchaseXml)
// called when: invalid product identifier specified when requesting product data,
//              called once for each invalid product identifier.
//              purchaseXml string will be of the format
//              <shop_item_purchase_success product_identifier='itunes connect product identifier'
//                                          transaction_identifier='successful appstore transaction identifier' />
//
// gameObject.OnAppstoreProductPurchaseCancelled(productIdentifier)
// called when: user cancels the transaction
//
// gameObject.OnAppstoreProductPurchaseFailed(errorXml)
// called when: an error occurs during the purchase transaction.
//              errorXml string will be of the format
//              <shop_item_purchase_failure product_identifier='itunes connect product identifier'
//                                          code='integer error code'
//                                          description='localised error description'
//                                          failureReason='localised failure reason' />
//
// Based on the sample code in the official In-App Purchase Programming Guide.
// @see http://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/AddingaStoretoYourApplication/AddingaStoretoYourApplication.html
@implementation RoarSKStore

@synthesize unityListenerName;
@synthesize observer;

// A user can disable the ability to make in app purchases.
// Use this function to check if purchases can be made.
- (bool)purchasesEnabled {
	if([SKPaymentQueue canMakePayments]) {
		return true;
	} else {
		return false;
	}
}

- (RoarSKStore*)init:(const char*) listenerGameObject {
	[self setUnityListener: listenerGameObject];
	observer = [[RoarSKStoreObserver alloc] initWithRoarSKStore:self];
	[[SKPaymentQueue defaultQueue] addTransactionObserver:observer];
    return self;
}

- (void)setUnityListener:(const char*) listenerGameObject {
	unityListenerName = [[NSString alloc] initWithUTF8String:listenerGameObject];
}

// use the product ids to get product details from the appstore
// cProductIdentifiers is a comma-seperated list
- (void)requestProductData:(const char*) cProductIdentifiers {

	NSString *productIdentifiers = [NSString stringWithUTF8String:cProductIdentifiers];
	NSArray  *productIdentifiersList = [productIdentifiers componentsSeparatedByString:@","];

	// create a product data request object with the product identifiers
	SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithArray: productIdentifiersList]];
    // this class will handle the response
    request.delegate = self;
    // fire off the request
    [request start];
}

// Handles the response from an SKProductsRequest.
// Translates products to xml and sends them to a unity game object listening for in app purchase activity.
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSArray *products = response.products;

    // let's build the xml payload intended for the unity app
    // using a simple string append approach
    // the problem here is that we don't have xml escaping out of the box like we would
    // if we used an xml library
    NSMutableString *productDataXml = [NSMutableString stringWithString:@"<appstore>"];

    if([products count] > 0) {

	    // the price formatter is used to provide two versions of the price as xml attributes
	    //           @price : contains the price as a float
	    // @formatted_price : contains the price as a float plus a locale specific currency suffix
	    NSNumberFormatter *priceFormatter = [[[NSNumberFormatter alloc] init] autorelease];
		[priceFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];

	    // represent each product as a <shop_item> that can be processed by the unity app
	    for(int p=0;p<[products count];p++)
		{
			SKProduct *product = [products objectAtIndex:p];
			[priceFormatter setLocale:product.priceLocale];
			// TODO: escape the xml
			[productDataXml appendString:
				[NSString stringWithFormat:@"<shop_item product_identifier=\"%@\" title=\"%@\" description=\"%@\" price=\"%@\" price_formatted=\"%@\" />",
					[product productIdentifier],
					[product localizedTitle],
					[product localizedDescription],
					[[product price] stringValue],
					[priceFormatter stringFromNumber:product.price]]
			];
		}
    }

    [productDataXml appendString: @"</appstore>"];
    [self informUnityListener:@"OnAppstoreProductData" withMessage:productDataXml];

	// inform the unity app of any bad product ids - this can happen when:
	// - a product id is setup in roar but it does not exist in itunes connect.
	// - a product id is setup in roar but it does not exactly match the product id in itunes connect.
	for(NSString *invalidProductId in response.invalidProductIdentifiers) {
        [self informUnityListener:@"OnAppstoreRequestProductDataInvalidProductId" withMessage:invalidProductId];
	}
}

// purchase the appstore product, defaults to quantity 1
- (void)purchaseProduct:(const char*) productIdentifier {
	[self purchaseProduct: productIdentifier withQuantity: 1];
}

// purchase an appstore product
- (void)purchaseProduct:(const char*) cProductIdentifier withQuantity:(int)quantity {
	NSString *productIdentifier = [NSString stringWithUTF8String:cProductIdentifier];
	SKMutablePayment *payment = [SKMutablePayment paymentWithProductIdentifier:productIdentifier];
	payment.quantity = quantity;
	[[SKPaymentQueue defaultQueue] addPayment:payment];
}

//
- (void)onCompleteTransaction:(SKPaymentTransaction *)transaction forProduct:(NSString *)productIdentifier {

	NSString *productPurchasedXml =
		[NSString stringWithFormat:@"<shop_item_purchase_success product_identifier=\"%@\" transaction_identifier=\"%@\" />",
			productIdentifier,
			[transaction transactionIdentifier]];
	[self informUnityListener:@"OnAppstoreProductPurchaseComplete" withMessage:productPurchasedXml];
}

- (void)onFailedTransaction:(SKPaymentTransaction *)transaction forProduct:(NSString *)productIdentifier {
	NSError *error = transaction.error;
	if (error.code == SKErrorPaymentCancelled) {
        [self informUnityListener:@"OnAppstoreProductPurchaseCancelled" withMessage:productIdentifier];
    } else {
	    // TODO: escape the xml
		NSString *errorXml =
			[NSString stringWithFormat:@"<shop_item_purchase_failure product_identifier=\"%@\" code=\"%@\" description=\"%@\" failureReason=\"%@\" />",
				productIdentifier,
				[NSNumber numberWithInt:[error code]],
				[error localizedDescription],
				[error localizedFailureReason]];
		[self informUnityListener:@"OnAppstoreProductPurchaseFailed" withMessage:errorXml];
    }
}

// for asynchronous calls to RoarSKStore we use UnitySendMessage
// to transmit an xml message back to a game object in the unity app.
// this approach removes the need to poll for activity in the store kit transaction queue.
- (void)informUnityListener:(NSString *) methodName withMessage:(NSString *)message {
	UnitySendMessage([unityListenerName UTF8String], [methodName UTF8String], [message UTF8String]);
}

@end
