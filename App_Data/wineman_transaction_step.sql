CREATE DATABASE  IF NOT EXISTS `wineman` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `wineman`;
-- MySQL dump 10.13  Distrib 5.6.17, for Win32 (x86)
--
-- Host: localhost    Database: wineman
-- ------------------------------------------------------
-- Server version	5.6.20

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `transaction_step`
--

DROP TABLE IF EXISTS `transaction_step`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transaction_step` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `transaction_id` int(11) NOT NULL,
  `step_id` int(11) NOT NULL,
  `date` datetime DEFAULT NULL,
  `done` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=306 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transaction_step`
--

LOCK TABLES `transaction_step` WRITE;
/*!40000 ALTER TABLE `transaction_step` DISABLE KEYS */;
INSERT INTO `transaction_step` VALUES (66,50,1,'2014-11-29 20:31:04',''),(67,50,2,'2014-12-05 20:31:04','\0'),(68,50,5,'2014-12-17 20:31:04','\0'),(69,50,6,'2014-12-17 20:31:04','\0'),(70,50,11,'2014-12-24 20:31:04','\0'),(71,50,12,'2014-12-30 20:31:04','\0'),(72,51,1,'2014-11-29 20:31:38',''),(73,51,2,'2014-12-05 20:31:38','\0'),(74,51,5,'2014-12-17 20:31:38','\0'),(75,51,6,'2014-12-17 20:31:38','\0'),(76,51,8,'2014-11-29 20:31:38',''),(77,51,11,'2014-12-24 20:31:38','\0'),(78,51,12,'2014-12-30 20:31:38','\0'),(79,52,1,'2014-11-29 20:31:59',''),(80,52,2,'2014-12-13 20:31:59','\0'),(81,52,5,'2014-12-23 20:31:59','\0'),(82,52,6,'2014-12-23 20:31:59','\0'),(83,52,8,'2014-11-29 20:31:59',''),(84,52,11,'2015-01-06 20:31:59','\0'),(85,52,12,'2015-01-12 20:31:59','\0'),(86,53,1,'2014-11-30 10:14:42',''),(87,53,2,'2014-12-06 10:14:42','\0'),(88,53,5,'2014-12-18 10:14:42','\0'),(89,53,6,'2014-12-18 10:14:42','\0'),(90,53,11,'2014-12-25 10:14:42','\0'),(91,53,12,'2014-12-31 10:14:42','\0'),(92,54,1,'2014-11-30 17:06:38',''),(93,54,2,'2014-12-14 17:06:38','\0'),(94,54,5,'2014-12-24 17:06:38','\0'),(95,54,6,'2014-12-24 17:06:38','\0'),(96,54,11,'2014-12-31 17:06:38','\0'),(97,54,12,'2015-01-07 17:06:38','\0'),(98,55,1,'2014-12-10 20:55:19',''),(99,55,2,'2014-12-16 20:55:19','\0'),(100,55,5,'2014-12-28 20:55:19','\0'),(101,55,6,'2014-12-28 20:55:19','\0'),(102,55,11,'2015-01-04 20:55:19','\0'),(103,55,12,'2015-01-10 20:55:19','\0'),(104,56,1,'2014-12-10 21:17:38',''),(105,56,2,'2014-12-16 21:17:38','\0'),(106,56,5,'2014-12-28 21:17:38','\0'),(107,56,6,'2014-12-28 21:17:38','\0'),(108,56,11,'2015-01-04 21:17:38','\0'),(109,56,12,'2015-01-10 21:17:38','\0'),(110,57,1,'2014-12-13 14:27:27',''),(111,57,2,'2014-12-27 14:27:27','\0'),(112,57,5,'2015-01-06 14:27:27','\0'),(113,57,6,'2015-01-06 14:27:27','\0'),(114,57,8,'2014-12-13 14:27:27','\0'),(115,57,11,'2015-01-20 14:27:27','\0'),(116,57,12,'2015-01-26 14:27:27','\0'),(117,58,1,'2014-12-13 14:28:22',''),(118,58,2,'2014-12-19 14:28:22','\0'),(119,58,5,'2014-12-31 14:28:22','\0'),(120,58,6,'2014-12-31 14:28:22','\0'),(121,58,11,'2015-01-13 14:28:22','\0'),(122,58,12,'2015-01-20 14:28:22','\0'),(123,59,1,'2014-12-13 14:28:46',''),(124,59,2,'2014-12-27 14:28:46','\0'),(125,59,5,'2015-01-06 14:28:46','\0'),(126,59,6,'2015-01-06 14:28:46','\0'),(127,59,8,'2014-12-13 14:28:46','\0'),(128,59,11,'2015-01-13 14:28:46','\0'),(129,59,12,'2015-01-20 14:28:46','\0'),(130,60,1,'2014-12-13 14:29:17',''),(131,60,2,'2014-12-27 14:29:17','\0'),(132,60,5,'2015-01-06 14:29:17','\0'),(133,60,6,'2015-01-06 14:29:17','\0'),(134,60,8,'2014-12-13 14:29:17','\0'),(135,60,11,'2015-01-13 14:29:17','\0'),(136,60,12,'2015-01-20 14:29:17','\0'),(137,61,1,'2014-12-24 14:37:30',''),(138,61,2,'2015-01-07 14:37:30','\0'),(139,61,5,'2015-01-17 14:37:30','\0'),(140,61,6,'2015-01-17 14:37:30','\0'),(141,61,8,'2014-12-24 14:37:30','\0'),(142,61,11,'2015-01-24 14:37:30','\0'),(143,61,12,'2015-01-31 14:37:30','\0'),(144,62,1,'2014-12-24 14:47:48',''),(145,62,2,'2014-12-30 14:47:48','\0'),(146,62,5,'2015-01-11 14:47:48','\0'),(147,62,6,'2015-01-11 14:47:48','\0'),(148,62,8,'2014-12-24 14:47:48','\0'),(149,62,11,'2015-01-18 14:47:48','\0'),(150,62,12,'2015-01-24 14:47:48','\0'),(151,63,1,'2014-12-24 15:00:52',''),(152,63,2,'2014-12-30 15:00:52','\0'),(153,63,5,'2015-01-11 15:00:52','\0'),(154,63,6,'2015-01-11 15:00:52','\0'),(155,63,8,'2014-12-24 15:00:52','\0'),(156,63,11,'2015-01-18 15:00:52','\0'),(157,63,12,'2015-01-24 15:00:52','\0'),(158,64,1,'2014-12-24 15:14:01',''),(159,64,2,'2014-12-30 15:14:01','\0'),(160,64,5,'2015-01-11 15:14:01','\0'),(161,64,6,'2015-01-11 15:14:01','\0'),(162,64,8,'2014-12-24 15:14:01','\0'),(163,64,11,'2015-01-18 15:14:01','\0'),(164,64,12,'2015-01-24 15:14:01','\0'),(165,65,1,'2014-12-24 15:16:40',''),(166,65,2,'2014-12-30 15:16:40','\0'),(167,65,5,'2015-01-11 15:16:40','\0'),(168,65,6,'2015-01-11 15:16:40','\0'),(169,65,11,'2015-01-18 15:16:40','\0'),(170,65,12,'2015-01-24 15:16:40','\0'),(171,66,1,'2014-12-24 15:23:47',''),(172,66,2,'2015-01-07 15:23:47','\0'),(173,66,5,'2015-01-17 15:23:47','\0'),(174,66,6,'2015-01-17 15:23:47','\0'),(175,66,8,'2014-12-24 15:23:47','\0'),(176,66,11,'2015-01-24 15:23:47','\0'),(177,66,12,'2015-01-31 15:23:47','\0'),(178,67,1,'2014-12-24 15:31:32',''),(179,67,2,'2015-01-07 15:31:32','\0'),(180,67,5,'2015-01-17 15:31:32','\0'),(181,67,6,'2015-01-17 15:31:32','\0'),(182,67,8,'2014-12-24 15:31:32','\0'),(183,67,11,'2015-01-24 15:31:32','\0'),(184,67,12,'2015-01-31 15:31:32','\0'),(185,68,1,'2014-12-24 15:42:48',''),(186,68,2,'2014-12-30 15:42:48','\0'),(187,68,5,'2015-01-11 15:42:48','\0'),(188,68,6,'2015-01-11 15:42:48','\0'),(189,68,8,'2014-12-24 15:42:48','\0'),(190,68,11,'2015-01-18 15:42:48','\0'),(191,68,12,'2015-01-24 15:42:48','\0'),(192,69,1,'2014-12-24 15:48:35',''),(193,69,2,'2014-12-30 15:48:35','\0'),(194,69,5,'2015-01-11 15:48:35','\0'),(195,69,6,'2015-01-11 15:48:35','\0'),(196,69,8,'2014-12-24 15:48:35','\0'),(197,69,11,'2015-01-18 15:48:35','\0'),(198,69,12,'2015-01-24 15:48:35','\0'),(199,70,1,'2014-12-25 12:27:13',''),(200,70,2,'2014-12-31 12:27:13','\0'),(201,70,5,'2015-01-12 12:27:13','\0'),(202,70,6,'2015-01-12 12:27:13','\0'),(203,70,8,'2014-12-25 12:27:13','\0'),(204,70,11,'2015-01-19 12:27:13','\0'),(205,70,12,'2015-01-25 12:27:13','\0'),(206,71,1,'2014-12-25 12:35:11',''),(207,71,2,'2014-12-31 12:35:11','\0'),(208,71,5,'2015-01-12 12:35:11','\0'),(209,71,6,'2015-01-12 12:35:11','\0'),(210,71,8,'2014-12-25 12:35:11','\0'),(211,71,11,'2015-01-19 12:35:11','\0'),(212,71,12,'2015-01-25 12:35:11','\0'),(213,72,1,'2014-12-25 13:19:40',''),(214,72,2,'2014-12-31 13:19:40','\0'),(215,72,5,'2015-01-12 13:19:40','\0'),(216,72,6,'2015-01-12 13:19:40','\0'),(217,72,8,'2014-12-25 13:19:40','\0'),(218,72,11,'2015-01-19 13:19:40','\0'),(219,72,12,'2015-01-25 13:19:40','\0'),(220,73,1,'2014-12-26 13:22:38',''),(221,73,2,'2015-01-09 13:22:38','\0'),(222,73,5,'2015-01-19 13:22:38','\0'),(223,73,6,'2015-01-19 13:22:38','\0'),(224,73,8,'2014-12-26 13:22:38','\0'),(225,73,11,'2015-02-02 13:22:38','\0'),(226,73,12,'2015-02-08 13:22:38','\0'),(227,74,1,'2014-12-26 13:24:55',''),(228,74,2,'2015-01-01 13:24:55','\0'),(229,74,5,'2015-01-13 13:24:55','\0'),(230,74,6,'2015-01-13 13:24:55','\0'),(231,74,8,'2014-12-26 13:24:55','\0'),(232,74,11,'2015-01-20 13:24:55','\0'),(233,74,12,'2015-01-26 13:24:55','\0'),(234,75,1,'2014-12-26 13:26:44',''),(235,75,2,'2015-01-01 13:26:44','\0'),(236,75,5,'2015-01-13 13:26:44','\0'),(237,75,6,'2015-01-13 13:26:44','\0'),(238,75,8,'2014-12-26 13:26:44','\0'),(239,75,11,'2015-01-20 13:26:44','\0'),(240,75,12,'2015-01-26 13:26:44','\0'),(241,76,1,'2014-12-26 13:58:52',''),(242,76,2,'2015-01-01 13:58:52','\0'),(243,76,5,'2015-01-13 13:58:52','\0'),(244,76,6,'2015-01-13 13:58:52','\0'),(245,76,8,'2014-12-26 13:58:52','\0'),(246,76,11,'2015-01-20 13:58:52','\0'),(247,76,12,'2015-01-26 13:58:52','\0'),(248,77,1,'2015-01-01 13:12:49',''),(249,77,2,'2015-01-07 13:12:49','\0'),(250,77,5,'2015-01-19 13:12:49','\0'),(251,77,6,'2015-01-19 13:12:49','\0'),(252,77,11,'2015-01-26 13:12:49','\0'),(253,77,12,'2015-02-01 13:12:49','\0'),(254,78,1,'2015-01-05 21:15:19',''),(255,78,2,'2015-01-11 21:15:19','\0'),(256,78,5,'2015-01-23 21:15:19','\0'),(257,78,6,'2015-01-23 21:15:19','\0'),(258,78,8,'2015-01-05 21:15:19','\0'),(259,78,11,'2015-01-30 21:15:19','\0'),(260,78,12,'2015-02-05 21:15:19','\0'),(261,79,1,'2015-01-05 21:30:16',''),(262,79,2,'2015-01-11 21:30:16','\0'),(263,79,5,'2015-01-23 21:30:16','\0'),(264,79,6,'2015-01-23 21:30:16','\0'),(265,79,11,'2015-01-30 21:30:16','\0'),(266,79,12,'2015-02-05 21:30:16','\0'),(267,80,1,'2015-01-05 21:34:56',''),(268,80,2,'2015-01-11 21:34:56','\0'),(269,80,5,'2015-01-23 21:34:56','\0'),(270,80,6,'2015-01-23 21:34:56','\0'),(271,80,11,'2015-01-30 21:34:56','\0'),(272,80,12,'2015-02-05 21:34:56','\0'),(273,81,1,'2015-01-05 21:37:42',''),(274,81,2,'2015-01-19 21:37:42','\0'),(275,81,5,'2015-01-29 21:37:42','\0'),(276,81,6,'2015-01-29 21:37:42','\0'),(277,81,8,'2015-01-05 21:37:42','\0'),(278,81,11,'2015-02-05 21:37:42','\0'),(279,81,12,'2015-02-12 21:37:42','\0'),(280,82,1,'2015-01-10 13:08:45',''),(281,82,2,'2015-01-16 13:08:45','\0'),(282,82,5,'2015-01-28 13:08:45','\0'),(283,82,6,'2015-01-28 13:08:45','\0'),(284,82,8,'2015-01-10 13:08:45','\0'),(285,82,11,'2015-02-04 13:08:45','\0'),(286,82,12,'2015-02-10 13:08:45','\0'),(287,83,1,'2015-01-10 13:22:55',''),(288,83,2,'2015-01-16 13:22:55','\0'),(289,83,5,'2015-01-28 13:22:55','\0'),(290,83,6,'2015-01-28 13:22:55','\0'),(291,83,8,'2015-01-10 13:22:55','\0'),(292,83,11,'2015-02-04 13:22:55','\0'),(293,83,12,'2015-02-10 13:22:55','\0'),(294,84,1,'2015-01-10 14:39:55',''),(295,84,2,'2015-01-16 14:39:55','\0'),(296,84,5,'2015-01-28 14:39:55','\0'),(297,84,6,'2015-01-28 14:39:55','\0'),(298,84,11,'2015-02-04 14:39:55','\0'),(299,84,12,'2015-02-10 14:39:55','\0'),(300,85,1,'2015-01-11 11:36:33','\0'),(301,85,2,'2015-01-17 11:36:33','\0'),(302,85,5,'2015-01-29 11:36:33','\0'),(303,85,6,'2015-01-29 11:36:33','\0'),(304,85,11,'2015-02-05 11:36:33','\0'),(305,85,12,'2015-02-11 11:36:33','\0');
/*!40000 ALTER TABLE `transaction_step` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-01-11 12:46:27
